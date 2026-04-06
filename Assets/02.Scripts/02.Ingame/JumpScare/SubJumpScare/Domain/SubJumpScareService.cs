using UnityEngine;

public sealed class SubJumpScareService
{
    private readonly Object _logContext;
    private readonly PlayerController _playerController;
    private readonly TensionManager _tensionManager;
    private readonly Transform _playerRootTransform;
    private readonly Transform _playerCameraTransform;
    private readonly FakeEnemyJumpScareExecutor _fakeEnemyJumpScareExecutor;
    private readonly PostProcessSubJumpScareExecutor _postProcessExecutor;
    private readonly SoundSubJumpScareExecutor _soundExecutor;
    private readonly SubJumpScareDatabaseSO _database;

    private readonly bool _enableLog;
    private readonly bool _enableSelectionLog;
    private readonly bool _enableGuaranteeLog;

    private readonly GameEventPublisher _eventPublisher;
    private readonly SubJumpScareSelectionCoordinator _selectionCoordinator;

    private float _periodicTimer;

    public SubJumpScareService(
        Object logContext,
        PlayerController playerController,
        TensionManager tensionManager,
        Transform playerRootTransform,
        Transform playerCameraTransform,
        FakeEnemyJumpScareExecutor fakeEnemyJumpScareExecutor,
        PostProcessSubJumpScareExecutor postProcessExecutor,
        SoundSubJumpScareExecutor soundExecutor,
        SubJumpScareDatabaseSO database,
        bool enableLog,
        bool enableSelectionLog,
        bool enableGuaranteeLog)
    {
        _logContext = logContext;
        _playerController = playerController;
        _tensionManager = tensionManager;
        _playerRootTransform = playerRootTransform;
        _playerCameraTransform = playerCameraTransform;
        _fakeEnemyJumpScareExecutor = fakeEnemyJumpScareExecutor;
        _postProcessExecutor = postProcessExecutor;
        _soundExecutor = soundExecutor;
        _database = database;

        _enableLog = enableLog;
        _enableSelectionLog = enableSelectionLog;
        _enableGuaranteeLog = enableGuaranteeLog;

        _selectionCoordinator = new SubJumpScareSelectionCoordinator(
            new SubJumpScareCommonValidator(),
            new SubJumpScareCandidateCollector(),
            new SubJumpScareWeightedPicker(),
            new SubJumpScareCooldownState(),
            new SubJumpScareHistory());

        _eventPublisher = new GameEventPublisher();
        _eventPublisher.SetSource(logContext);
    }

    public void Dispose()
    {
    }

    public void Tick(
        float deltaTime,
        bool usePeriodicTick,
        float periodicInterval,
        bool isMainJumpScareRunning,
        bool isInMainEndGraceTime,
        bool isImportantVoicePlaying,
        bool isSonarAvailable,
        bool canPlaceFakeEnemyThisAttempt)
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
            isInMainEndGraceTime,
            isImportantVoicePlaying,
            isSonarAvailable,
            canPlaceFakeEnemyThisAttempt);
    }

    public void TrySelectPeriodic(
        bool isMainJumpScareRunning,
        bool isInMainEndGraceTime,
        bool isImportantVoicePlaying,
        bool isSonarAvailable,
        bool canPlaceFakeEnemyThisAttempt)
    {
        SubJumpScareContext context = CreateContext(
            isMainJumpScareRunning,
            isInMainEndGraceTime,
            isImportantVoicePlaying,
            isSonarAvailable,
            canPlaceFakeEnemyThisAttempt);

        SubJumpScareSelectionResult selectedResult = _selectionCoordinator.SelectPeriodic(_database, context);
        SubJumpScareSelectionResult finalResult = ResolvePeriodicSelectionResult(selectedResult);

        LogResult(context, finalResult);
        PublishRawResult(finalResult);
    }

    public void TrySelectSonar(
        bool isMainJumpScareRunning,
        bool isInMainEndGraceTime,
        bool isImportantVoicePlaying,
        bool isSonarAvailable,
        bool canPlaceFakeEnemyThisAttempt)
    {
        SubJumpScareContext context = CreateContext(
            isMainJumpScareRunning,
            isInMainEndGraceTime,
            isImportantVoicePlaying,
            isSonarAvailable,
            canPlaceFakeEnemyThisAttempt);

        SubJumpScareSelectionResult selectedResult = _selectionCoordinator.SelectSonar(_database, context);
        SubJumpScareSelectionResult finalResult = ResolveSonarSelectionResult(selectedResult);

        if (_selectionCoordinator.FakeEnemyGuaranteePending
            && _enableGuaranteeLog
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

        if (CanExecuteFakeEnemy(fakeEnemyDefinition) == false)
        {
            _selectionCoordinator.KeepFakeEnemyGuaranteePending();

            return SubJumpScareSelectionResult.CreateFail(
                ESubJumpScareTriggerType.Sonar,
                "Fake enemy execution references are invalid.");
        }

        FakeEnemyJumpScareExecuteRequest executeRequest = CreateFakeEnemyExecuteRequest(fakeEnemyDefinition);
        if (executeRequest.IsValid() == false)
        {
            _selectionCoordinator.KeepFakeEnemyGuaranteePending();

            return SubJumpScareSelectionResult.CreateFail(
                ESubJumpScareTriggerType.Sonar,
                "Fake enemy execution request is invalid.");
        }

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
            && _playerRootTransform != null
            && _playerCameraTransform != null;
    }

    private FakeEnemyJumpScareExecuteRequest CreateFakeEnemyExecuteRequest(
        FakeEnemySubJumpScareDefinitionSO fakeEnemyDefinition)
    {
        return new FakeEnemyJumpScareExecuteRequest(
            _playerCameraTransform.position,
            _playerCameraTransform.forward,
            _playerRootTransform,
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
            && _playerRootTransform != null
            && _playerCameraTransform != null
            && SoundManager.Instance != null;
    }

    private SoundJumpScareExecutionRequest CreateSoundExecuteRequest(
        SoundSubJumpScareDefinitionSO soundDefinition)
    {
        return new SoundJumpScareExecutionRequest(
            soundDefinition,
            _playerRootTransform,
            _playerCameraTransform,
            SoundManager.Instance);
    }

    private void PublishRawResult(SubJumpScareSelectionResult result)
    {
        _eventPublisher.TryPublish(eventContext => new SubJumpScareTriggeredRawEvent(eventContext, result));
    }

    private SubJumpScareContext CreateContext(
        bool isMainJumpScareRunning,
        bool isInMainEndGraceTime,
        bool isImportantVoicePlaying,
        bool isSonarAvailable,
        bool canPlaceFakeEnemyThisAttempt)
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
        context.IsPostProcessActive = _postProcessExecutor != null && _postProcessExecutor.IsPlaying;
        context.IsImportantVoicePlaying = isImportantVoicePlaying;
        context.IsSonarAvailable = isSonarAvailable;
        context.CanPlaceFakeEnemyThisAttempt = canPlaceFakeEnemyThisAttempt;

        return context;
    }

    private void LogResult(SubJumpScareContext context, SubJumpScareSelectionResult result)
    {
        if (result.IsSuccess)
        {
            if (_enableSelectionLog == false)
            {
                return;
            }

            Debug.Log(
                $"[SubJumpScare] Selection Success | Trigger={result.TriggerType} | Type={result.Data.Type} | Intensity={result.Data.Intensity} | Tension={context.TotalTension} | Mode={context.CurrentPlayerInteractMode} | Id={result.Data.Id} | Name={result.Data.DisplayName}",
                _logContext);
            return;
        }

        if (_enableLog == false)
        {
            return;
        }

        Debug.Log(
            $"[SubJumpScare] Selection Fail | Trigger={result.TriggerType} | Tension={context.TotalTension} | Mode={context.CurrentPlayerInteractMode} | Reason={result.FailReason}",
            _logContext);
    }
}