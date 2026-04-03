using System;
using UnityEngine;

public class SubJumpScareManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TensionManager tensionManager;
    [SerializeField] private Transform playerRootTransform;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private FakeEnemyJumpScareExecutor fakeEnemyJumpScareExecutor;

    [Header("Data")]
    [SerializeField] private SubJumpScareDatabaseSO database;

    [Header("Periodic Trigger")]
    [SerializeField] private bool usePeriodicTick = true;
    [SerializeField] private float periodicInterval = 1f;

    [Header("Main Grace")]
    [SerializeField] private float mainEndGraceDuration = 2f;

    [Header("Runtime State")]
    [SerializeField] private bool isPostProcessActive;
    [SerializeField] private bool isImportantVoicePlaying;
    [SerializeField] private bool isSonarAvailable = true;
    [SerializeField] private bool canPlaceFakeEnemyThisAttempt = true;

    [Header("Debug")]
    [SerializeField] private bool enableLog = true;
    [SerializeField] private bool enableSelectionLog = true;
    [SerializeField] private bool enableGuaranteeLog = true;

    private float _periodicTimer;
    private float _mainGraceRemainingTime;
    private bool _isMainJumpScareRunning;

    private GameEventPublisher _eventPublisher;

    private SubJumpScareCooldownState _cooldownState;
    private SubJumpScareHistory _history;
    private SubJumpScareCommonValidator _commonValidator;
    private SubJumpScareCandidateCollector _candidateCollector;
    private SubJumpScareWeightedPicker _weightedPicker;
    private SubJumpScareSelectionCoordinator _selectionCoordinator;

    private void Awake()
    {
        _cooldownState = new SubJumpScareCooldownState();
        _history = new SubJumpScareHistory();
        _commonValidator = new SubJumpScareCommonValidator();
        _candidateCollector = new SubJumpScareCandidateCollector();
        _weightedPicker = new SubJumpScareWeightedPicker();

        _selectionCoordinator = new SubJumpScareSelectionCoordinator(
            _commonValidator,
            _candidateCollector,
            _weightedPicker,
            _cooldownState,
            _history);

        _eventPublisher = new GameEventPublisher();
        _eventPublisher.SetSource(this);

        GameEventHub.Instance.Subscribe<SonarScanStartedRawEvent>(OnSonarActive);
    }

    private void OnSonarActive(SonarScanStartedRawEvent data)
    {
        TrySelectSonar();
    }

    private void Update()
    {
        UpdateMainGraceTime();

        if (usePeriodicTick == false)
        {
            return;
        }

        _periodicTimer += Time.deltaTime;

        if (_periodicTimer >= periodicInterval)
        {
            _periodicTimer = 0f;
            TrySelectPeriodic();
        }
    }

    public void SetMainJumpScareRunning(bool isRunning)
    {
        _isMainJumpScareRunning = isRunning;

        if (isRunning == false)
        {
            _mainGraceRemainingTime = mainEndGraceDuration;
        }
    }

    [ContextMenu("Debug/Try Select Periodic")]
    public void TrySelectPeriodic()
    {
        SubJumpScareContext context = CreateContext();
        SubJumpScareSelectionResult result = _selectionCoordinator.SelectPeriodic(database, context);

        LogResult(context, result);
        PublishRawResult(result);
    }

    [ContextMenu("Debug/Try Select Sonar")]
    public void TrySelectSonar()
    {
        SubJumpScareContext context = CreateContext();
        SubJumpScareSelectionResult selectedResult = _selectionCoordinator.SelectSonar(database, context);
        SubJumpScareSelectionResult finalResult = ResolveSonarSelectionResult(selectedResult);

        if (_selectionCoordinator.FakeEnemyGuaranteePending == true
            && enableGuaranteeLog == true
            && finalResult.IsSuccess == false)
        {
            Debug.Log("[SubJumpScare] 가짜적 배치 실패. 다음 소나 시도에서도 계속 재검사합니다.");
        }

        LogResult(context, finalResult);
        PublishRawResult(finalResult);
    }

    private SubJumpScareSelectionResult ResolveSonarSelectionResult(SubJumpScareSelectionResult selectedResult)
    {
        if (selectedResult.IsSuccess == false)
        {
            return selectedResult;
        }

        if (selectedResult.Data == null)
        {
            return selectedResult;
        }

        if (selectedResult.Data.Type != ESubJumpScareType.FakeEnemy)
        {
            _selectionCoordinator.ConfirmSonarTriggered(selectedResult);
            return selectedResult;
        }

        FakeEnemySubJumpScareDefinitionSO fakeEnemyDefinition;

        if (database == null || database.TryGetFakeEnemyDefinition(selectedResult.Data.Id, out fakeEnemyDefinition) == false)
        {
            _selectionCoordinator.KeepFakeEnemyGuaranteePending();

            return SubJumpScareSelectionResult.CreateFail(
                ESubJumpScareTriggerType.Sonar,
                "선택된 가짜적 정의를 찾지 못했습니다.");
        }

        if (CanExecuteFakeEnemy(fakeEnemyDefinition) == false)
        {
            _selectionCoordinator.KeepFakeEnemyGuaranteePending();

            return SubJumpScareSelectionResult.CreateFail(
                ESubJumpScareTriggerType.Sonar,
                "가짜적 실행 참조가 올바르게 설정되지 않았습니다.");
        }

        FakeEnemyJumpScareExecuteRequest executeRequest = CreateFakeEnemyExecuteRequest(fakeEnemyDefinition);

        if (executeRequest.IsValid() == false)
        {
            _selectionCoordinator.KeepFakeEnemyGuaranteePending();

            return SubJumpScareSelectionResult.CreateFail(
                ESubJumpScareTriggerType.Sonar,
                "가짜적 실행 요청 데이터가 유효하지 않습니다.");
        }

        FakeEnemyInstance spawnedInstance;
        bool isSpawned = fakeEnemyJumpScareExecutor.TryExecute(executeRequest, out spawnedInstance);

        if (isSpawned == false)
        {
            _selectionCoordinator.KeepFakeEnemyGuaranteePending();

            return SubJumpScareSelectionResult.CreateFail(
                ESubJumpScareTriggerType.Sonar,
                "가짜적 위치 선정 또는 생성에 실패했습니다.");
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

        if (fakeEnemyJumpScareExecutor == null)
        {
            return false;
        }

        if (playerRootTransform == null)
        {
            return false;
        }

        if (playerCameraTransform == null)
        {
            return false;
        }

        return true;
    }

    private FakeEnemyJumpScareExecuteRequest CreateFakeEnemyExecuteRequest(
        FakeEnemySubJumpScareDefinitionSO fakeEnemyDefinition)
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

    private void PublishRawResult(SubJumpScareSelectionResult result)
    {
        _eventPublisher.TryPublish(
            eventContext => new SubJumpScareTriggeredRawEvent(eventContext, result));
    }

    private SubJumpScareContext CreateContext()
    {
        SubJumpScareContext context = new SubJumpScareContext();

        if (playerController != null)
        {
            context.CurrentPlayerInteractMode = playerController.InteractMode;
        }

        if (tensionManager != null)
        {
            context.TotalTension = tensionManager.TotalTension;
        }

        context.IsMainJumpScareRunning = _isMainJumpScareRunning;
        context.IsInMainEndGraceTime = _mainGraceRemainingTime > 0f;

        context.IsPostProcessActive = isPostProcessActive;
        context.IsImportantVoicePlaying = isImportantVoicePlaying;

        context.IsSonarAvailable = isSonarAvailable;
        context.CanPlaceFakeEnemyThisAttempt = canPlaceFakeEnemyThisAttempt;

        return context;
    }

    private void UpdateMainGraceTime()
    {
        if (_mainGraceRemainingTime <= 0f)
        {
            return;
        }

        _mainGraceRemainingTime -= Time.deltaTime;

        if (_mainGraceRemainingTime < 0f)
        {
            _mainGraceRemainingTime = 0f;
        }
    }

    private void LogResult(SubJumpScareContext context, SubJumpScareSelectionResult result)
    {
        if (result.IsSuccess == true)
        {
            if (enableSelectionLog == false)
            {
                return;
            }

            Debug.Log(
                $"[SubJumpScare] Selection Success | Trigger={result.TriggerType} | Type={result.Data.Type} | Intensity={result.Data.Intensity} | Tension={context.TotalTension} | Mode={context.CurrentPlayerInteractMode} | Id={result.Data.Id} | Name={result.Data.DisplayName}");
            return;
        }

        if (enableLog == false)
        {
            return;
        }

        Debug.Log(
            $"[SubJumpScare] Selection Fail | Trigger={result.TriggerType} | Tension={context.TotalTension} | Mode={context.CurrentPlayerInteractMode} | Reason={result.FailReason}");
    }
}

