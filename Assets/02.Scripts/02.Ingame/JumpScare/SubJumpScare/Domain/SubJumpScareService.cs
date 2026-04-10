using System.Collections.Generic;
using UnityEngine;

public sealed class SubJumpScareService
{
    private readonly JumpScareManager _owner;
    private readonly Object _logContext;
    private readonly PlayerController _playerController;
    private readonly TensionManager _tensionManager;
    private readonly FakeEnemyJumpScareExecutor _fakeEnemyJumpScareExecutor;
    private readonly PostProcessSubJumpScareExecutor _postProcessExecutor;
    private readonly SoundSubJumpScareExecutor _soundExecutor;
    private readonly SubJumpScareDatabaseSO _database;

    private readonly GameEventPublisher _eventPublisher;
    private readonly SubJumpScareSelectionCoordinator _selectionCoordinator;

    private float _periodicTimer;

    public SubJumpScareService(
        JumpScareManager owner,
        PlayerController playerController,
        TensionManager tensionManager,
        FakeEnemyJumpScareExecutor fakeEnemyJumpScareExecutor,
        PostProcessSubJumpScareExecutor postProcessExecutor,
        SoundSubJumpScareExecutor soundExecutor,
        SubJumpScareDatabaseSO database)
    {
        _owner = owner;
        _logContext = owner;
        _playerController = playerController;
        _tensionManager = tensionManager;
        _fakeEnemyJumpScareExecutor = fakeEnemyJumpScareExecutor;
        _postProcessExecutor = postProcessExecutor;
        _soundExecutor = soundExecutor;
        _database = database;

        _selectionCoordinator = new SubJumpScareSelectionCoordinator(
            new SubJumpScareCommonValidator(),
            new SubJumpScareCandidateCollector(),
            new SubJumpScareWeightedPicker(),
            new SubJumpScareCooldownState(),
            new SubJumpScareHistory());

        _eventPublisher = new GameEventPublisher();
        _eventPublisher.SetSource(owner);
    }

    public void Dispose()
    {
    }

    public float DebugGlobalCooldownRemaining => _selectionCoordinator.GlobalCooldownRemaining;
    public float DebugSoundCooldownRemaining => _selectionCoordinator.GetTypeCooldownRemaining(ESubJumpScareType.Sound);
    public float DebugPostProcessCooldownRemaining => _selectionCoordinator.GetTypeCooldownRemaining(ESubJumpScareType.PostProcess);
    public float DebugFakeEnemyCooldownRemaining => _selectionCoordinator.GetTypeCooldownRemaining(ESubJumpScareType.FakeEnemy);

    public List<SubJumpScareItemCooldownDebugInfo> GetDebugItemCooldowns()
    {
        return _selectionCoordinator.GetActiveItemCooldowns();
    }

    public void Tick(
        float deltaTime,
        bool usePeriodicTick,
        float periodicInterval,
        bool isMainJumpScareRunning,
        bool isInMainEndGraceTime)
    {
        if (usePeriodicTick == false)
        {
            return;
        }

        _periodicTimer += deltaTime;
        if (_periodicTimer < periodicInterval)
        {
            return;
        }

        _periodicTimer = 0f;

        TrySelectPeriodic(
            isMainJumpScareRunning,
            isInMainEndGraceTime);
    }

    public void TrySelectPeriodic(
        bool isMainJumpScareRunning,
        bool isInMainEndGraceTime)
    {
        SubJumpScareContext context = CreateContext(
            isMainJumpScareRunning,
            isInMainEndGraceTime);

        SubJumpScareSelectionResult selectedResult = _selectionCoordinator.SelectPeriodic(_database, context);
        SubJumpScareSelectionResult finalResult = ResolvePeriodicSelectionResult(selectedResult);

        LogResult(context, finalResult);
        PublishRawResult(finalResult);
    }

    public void TrySelectSonar(
        bool isMainJumpScareRunning,
        bool isInMainEndGraceTime)
    {
        SubJumpScareContext context = CreateContext(
            isMainJumpScareRunning,
            isInMainEndGraceTime);

        SubJumpScareSelectionResult selectedResult = _selectionCoordinator.SelectSonar(_database, context);
        SubJumpScareSelectionResult finalResult = ResolveSonarSelectionResult(selectedResult);

        if (_selectionCoordinator.FakeEnemyGuaranteePending
            && IsSubLogEnabled()
            && finalResult.IsSuccess == false)
        {
            Debug.Log("[SubJumpScare] Fake enemy placement failed. Retry will remain pending.", _logContext);
        }

        LogResult(context, finalResult);
        PublishRawResult(finalResult);
    }

    private SubJumpScareSelectionResult ResolvePeriodicSelectionResult(SubJumpScareSelectionResult selectedResult)
    {
        if (selectedResult.IsSuccess == false || selectedResult.Data == null)
        {
            return selectedResult;
        }

        if (selectedResult.Data.Type == ESubJumpScareType.PostProcess)
        {
            if (_database == null
                || _database.TryGetPostProcessDefinition(
                    selectedResult.Data.Id,
                    out PostProcessSubJumpScareDefinitionSO definition) == false)
            {
                return SubJumpScareSelectionResult.CreateFail(
                    ESubJumpScareTriggerType.Periodic,
                    "Selected post process definition was not found.");
            }

            if (_postProcessExecutor == null)
            {
                return SubJumpScareSelectionResult.CreateFail(
                    ESubJumpScareTriggerType.Periodic,
                    "Post process executor is not assigned.");
            }

            bool isExecuted = _postProcessExecutor.TryExecute(definition);
            if (isExecuted == false)
            {
                return SubJumpScareSelectionResult.CreateFail(
                    ESubJumpScareTriggerType.Periodic,
                    "Post process execution failed.");
            }

            _selectionCoordinator.ConfirmPeriodicTriggered(selectedResult);
            return selectedResult;
        }

        if (selectedResult.Data.Type == ESubJumpScareType.Sound)
        {
            if (_database == null
                || _database.TryGetSoundDefinition(
                    selectedResult.Data.Id,
                    out SoundSubJumpScareDefinitionSO soundDefinition) == false)
            {
                return SubJumpScareSelectionResult.CreateFail(
                    ESubJumpScareTriggerType.Periodic,
                    "Selected sound definition was not found.");
            }

            if (CanExecuteSound(soundDefinition) == false)
            {
                return SubJumpScareSelectionResult.CreateFail(
                    ESubJumpScareTriggerType.Periodic,
                    "Sound execution references are invalid.");
            }

            SoundJumpScareExecutionRequest executeRequest = CreateSoundExecuteRequest(soundDefinition);
            if (executeRequest.IsValid() == false)
            {
                return SubJumpScareSelectionResult.CreateFail(
                    ESubJumpScareTriggerType.Periodic,
                    "Sound execution request is invalid.");
            }

            SoundJumpScareExecutionResult executeResult = _soundExecutor.TryExecute(executeRequest);
            if (executeResult.IsSuccess == false)
            {
                return SubJumpScareSelectionResult.CreateFail(
                    ESubJumpScareTriggerType.Periodic,
                    executeResult.Reason);
            }

            _selectionCoordinator.ConfirmPeriodicTriggered(selectedResult);
            return selectedResult;
        }

        return SubJumpScareSelectionResult.CreateFail(
            ESubJumpScareTriggerType.Periodic,
            "Unsupported type was selected for periodic trigger.");
    }

    private SubJumpScareSelectionResult ResolveSonarSelectionResult(SubJumpScareSelectionResult selectedResult)
    {
        if (selectedResult.IsSuccess == false || selectedResult.Data == null)
        {
            return selectedResult;
        }

        if (selectedResult.Data.Type != ESubJumpScareType.FakeEnemy)
        {
            _selectionCoordinator.ConfirmSonarTriggered(selectedResult);
            return selectedResult;
        }

        if (_database == null
            || _database.TryGetFakeEnemyDefinition(
                selectedResult.Data.Id,
                out FakeEnemySubJumpScareDefinitionSO fakeEnemyDefinition) == false)
        {
            _selectionCoordinator.KeepFakeEnemyGuaranteePending();

            return SubJumpScareSelectionResult.CreateFail(
                ESubJumpScareTriggerType.Sonar,
                "Selected fake enemy definition was not found.");
        }

        Transform playerRootTransform;
        Transform playerCameraTransform;

        if (CanExecuteFakeEnemy(fakeEnemyDefinition) == false
            || TryResolveRuntimeReferences(out playerRootTransform, out playerCameraTransform) == false)
        {
            _selectionCoordinator.KeepFakeEnemyGuaranteePending();

            return SubJumpScareSelectionResult.CreateFail(
                ESubJumpScareTriggerType.Sonar,
                "Fake enemy execution references are invalid.");
        }

        FakeEnemyJumpScareExecuteRequest executeRequest = CreateFakeEnemyExecuteRequest(
            fakeEnemyDefinition,
            playerRootTransform,
            playerCameraTransform);

        if (executeRequest.IsValid() == false)
        {
            _selectionCoordinator.KeepFakeEnemyGuaranteePending();

            return SubJumpScareSelectionResult.CreateFail(
                ESubJumpScareTriggerType.Sonar,
                "Fake enemy execution request is invalid.");
        }

        LogFakeEnemyExecuteRequest(
            fakeEnemyDefinition,
            executeRequest,
            playerRootTransform,
            playerCameraTransform);

        bool isSpawned = _fakeEnemyJumpScareExecutor.TryExecute(executeRequest, out _);
        if (isSpawned == false)
        {
            _selectionCoordinator.KeepFakeEnemyGuaranteePending();

            return SubJumpScareSelectionResult.CreateFail(
                ESubJumpScareTriggerType.Sonar,
                "Fake enemy placement or spawn failed.");
        }

        _selectionCoordinator.ConfirmSonarTriggered(selectedResult);
        return selectedResult;
    }

    private bool CanExecuteFakeEnemy(FakeEnemySubJumpScareDefinitionSO fakeEnemyDefinition)
    {
        if (fakeEnemyDefinition == null)
        {
            return false;
        }

        return _fakeEnemyJumpScareExecutor != null
            && ResolvePlayerRootTransform() != null
            && ResolvePlayerCameraTransform() != null;
    }

    private FakeEnemyJumpScareExecuteRequest CreateFakeEnemyExecuteRequest(
        FakeEnemySubJumpScareDefinitionSO fakeEnemyDefinition,
        Transform playerRootTransform,
        Transform playerCameraTransform)
    {
        return new FakeEnemyJumpScareExecuteRequest(
            playerCameraTransform.position,
            playerCameraTransform.forward,
            playerRootTransform,
            fakeEnemyDefinition.MinSpawnDistance,
            fakeEnemyDefinition.MaxSpawnDistance,
            fakeEnemyDefinition.AllowedForwardAngle,
            fakeEnemyDefinition.PosePrefabs,
            false,
            0);
    }

    private bool CanExecuteSound(SoundSubJumpScareDefinitionSO soundDefinition)
    {
        if (soundDefinition == null)
        {
            return false;
        }

        return _soundExecutor != null
            && ResolvePlayerRootTransform() != null
            && ResolvePlayerCameraTransform() != null
            && SoundManager.Instance != null;
    }

    private SoundJumpScareExecutionRequest CreateSoundExecuteRequest(
        SoundSubJumpScareDefinitionSO soundDefinition)
    {
        Transform playerRootTransform;
        Transform playerCameraTransform;

        if (TryResolveRuntimeReferences(out playerRootTransform, out playerCameraTransform) == false)
        {
            return default;
        }

        return new SoundJumpScareExecutionRequest(
            soundDefinition,
            playerRootTransform,
            playerCameraTransform,
            SoundManager.Instance);
    }

    private void LogFakeEnemyExecuteRequest(
        FakeEnemySubJumpScareDefinitionSO fakeEnemyDefinition,
        FakeEnemyJumpScareExecuteRequest executeRequest,
        Transform playerRootTransform,
        Transform playerCameraTransform)
    {
        if (IsSubLogEnabled() == false)
        {
            return;
        }

        string definitionId = fakeEnemyDefinition == null ? "null" : fakeEnemyDefinition.Common.Id;
        string definitionName = fakeEnemyDefinition == null ? "null" : fakeEnemyDefinition.Common.DisplayName;
        Vector3 playerPosition = playerRootTransform == null ? Vector3.zero : playerRootTransform.position;
        Vector3 cameraPosition = playerCameraTransform == null ? Vector3.zero : playerCameraTransform.position;
        Vector3 cameraForward = playerCameraTransform == null ? Vector3.zero : playerCameraTransform.forward;

        Debug.Log(
            $"[SubJumpScare] FakeEnemy Execute Request | Id={definitionId} | Name={definitionName} | " +
            $"PlayerPos={playerPosition} | CameraPos={cameraPosition} | CameraForward={cameraForward} | " +
            $"MinDist={executeRequest.MinDistance} | MaxDist={executeRequest.MaxDistance} | AllowedAngle={executeRequest.AllowedForwardAngle}",
            _logContext);
    }

    private void PublishRawResult(SubJumpScareSelectionResult result)
    {
        if (result.IsSuccess == false)
        {
            return;
        }
        
        _eventPublisher.TryPublish(eventContext => new SubJumpScareTriggeredRawEvent(eventContext, result));
    }

    private SubJumpScareContext CreateContext(
        bool isMainJumpScareRunning,
        bool isInMainEndGraceTime)
    {
        SubJumpScareContext context = new();

        if (_playerController != null)
        {
            context.CurrentPlayerInteractMode = _playerController.InteractMode;
        }

        if (_tensionManager != null)
        {
            context.TotalTension = _tensionManager.TotalTension;
        }

        context.IsMainJumpScareRunning = isMainJumpScareRunning;
        context.IsInMainEndGraceTime = isInMainEndGraceTime;

        return context;
    }

    private void LogResult(SubJumpScareContext context, SubJumpScareSelectionResult result)
    {
        if (IsSubLogEnabled() == false)
        {
            return;
        }

        if (result.IsSuccess)
        {
            Debug.Log(
                $"[SubJumpScare] Selection Success | Trigger={result.TriggerType} | Type={result.Data.Type} | Intensity={result.Data.Intensity} | Tension={context.TotalTension} | Mode={context.CurrentPlayerInteractMode} | Id={result.Data.Id} | Name={result.Data.DisplayName}",
                _logContext);
        }
        else
        {
            Debug.Log(
                $"[SubJumpScare] Selection Fail | Trigger={result.TriggerType} | Tension={context.TotalTension} | Mode={context.CurrentPlayerInteractMode} | Reason={result.FailReason}",
                _logContext);
        }
    }

    private bool TryResolveRuntimeReferences(
        out Transform playerRootTransform,
        out Transform playerCameraTransform)
    {
        playerRootTransform = ResolvePlayerRootTransform();
        playerCameraTransform = ResolvePlayerCameraTransform();

        return playerRootTransform != null && playerCameraTransform != null;
    }

    private Transform ResolvePlayerRootTransform()
    {
        if (_owner != null && _owner.PlayerRootTransform != null)
        {
            return _owner.PlayerRootTransform;
        }

        if (_playerController != null)
        {
            return _playerController.transform;
        }

        return null;
    }

    private Transform ResolvePlayerCameraTransform()
    {
        if (_owner != null && _owner.PlayerCameraTransform != null)
        {
            return _owner.PlayerCameraTransform;
        }

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            return mainCamera.transform;
        }

        return null;
    }

    private bool IsSubLogEnabled()
    {
        if (_owner == null)
        {
            return false;
        }

        return _owner.EnableSubLog;
    }
}
