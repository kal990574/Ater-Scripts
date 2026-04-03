using UnityEngine;

public class SubJumpScareManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TensionManager tensionManager;

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

    public void SetPostProcessActive(bool isActive)
    {
        isPostProcessActive = isActive;
    }

    public void SetImportantVoicePlaying(bool isPlaying)
    {
        isImportantVoicePlaying = isPlaying;
    }

    public void SetSonarAvailable(bool isAvailable)
    {
        isSonarAvailable = isAvailable;
    }

    public void SetCanPlaceFakeEnemyThisAttempt(bool canPlace)
    {
        canPlaceFakeEnemyThisAttempt = canPlace;
    }

    [ContextMenu("Debug/Try Select Periodic")]
    public void TrySelectPeriodic()
    {
        SubJumpScareContext context = CreateContext();
        SubJumpScareSelectionResult result = _selectionCoordinator.SelectPeriodic(database, context);
        LogResult(context, result);
        
        _eventPublisher.TryPublish(
            context => new SubJumpScareTriggeredRawEvent(context, result));
    }

    [ContextMenu("Debug/Try Select Sonar")]
    public void TrySelectSonar()
    {
        SubJumpScareContext context = CreateContext();
        SubJumpScareSelectionResult result = _selectionCoordinator.SelectSonar(database, context);

        if (_selectionCoordinator.FakeEnemyGuaranteePending == true && enableGuaranteeLog == true && result.IsSuccess == false)
        {
            Debug.Log("[SubJumpScare] 가짜적 배치 실패. 다음 소나 시도에서도 계속 재검사합니다.");
        }

        LogResult(context, result);
        
        _eventPublisher.TryPublish(
            context => new SubJumpScareTriggeredRawEvent(context, result));
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