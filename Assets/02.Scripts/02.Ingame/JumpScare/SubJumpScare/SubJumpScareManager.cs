using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SubJumpScareManager : MonoBehaviour
{
    private static SubJumpScareManager _instance;
    public static SubJumpScareManager Instance => _instance;
    
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TensionManager tensionManager;
    [SerializeField] private Transform playerRootTransform;
    [SerializeField] private Transform playerCameraTransform;

    
    [Title("Sub JumpScare")]
    [Header("Executors")]
    [SerializeField] private FakeEnemyJumpScareExecutor fakeEnemyJumpScareExecutor;
    [SerializeField] private PostProcessSubJumpScareExecutor _postProcessExecutor;

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
    
    private GameEventPublisher _eventPublisher;

    private SubJumpScareCooldownState _cooldownState;
    private SubJumpScareHistory _history;
    private SubJumpScareCommonValidator _commonValidator;
    private SubJumpScareCandidateCollector _candidateCollector;
    private SubJumpScareWeightedPicker _weightedPicker;
    private SubJumpScareSelectionCoordinator _selectionCoordinator;
    
    [Title("Main JumpScare")]
    
    [Header("Debug")]
    [SerializeField] private bool _enableDebugLog = true;
    [SerializeField] private bool _includeInactiveOnRegister = true;

    
    private readonly Dictionary<string, MainJumpScareBase> _mainJumpScareById = new Dictionary<string, MainJumpScareBase>();
    private readonly HashSet<string> _playingMainJumpScareIds = new HashSet<string>();
    
    private float _periodicTimer;
    private float _mainGraceRemainingTime;
    private bool _isMainJumpScareRunning;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        Init();
    }

    private void Init()
    {
        //서브 점프스케어 세팅
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
        
        //메인점프스케어 세팅
        RegisterSceneMainJumpScares();

        //이벤트 버스
        _eventPublisher = new GameEventPublisher();
        _eventPublisher.SetSource(this);

        //구독 연결
        GameEventHub.Instance.Subscribe<SonarScanStartedRawEvent>(OnSonarActive);
    }

    private void OnSonarActive(SonarScanStartedRawEvent data)
    {
        TrySelectSonar();
    }

    private void Update()
    {
        UpdateMainGraceTime();
        UpdateRuntimeStates();

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

    public void RegisterSceneMainJumpScares()
    {
        _mainJumpScareById.Clear();
        _playingMainJumpScareIds.Clear();

#if UNITY_2023_1_OR_NEWER
        FindObjectsInactive findObjectsInactive = _includeInactiveOnRegister == true
            ? FindObjectsInactive.Include
            : FindObjectsInactive.Exclude;

        MainJumpScareBase[] mainJumpScares = FindObjectsByType<MainJumpScareBase>(findObjectsInactive, FindObjectsSortMode.None);
#else
        MainJumpScareBase[] mainJumpScares = FindObjectsOfType<MainJumpScareBase>(_includeInactiveOnRegister);
#endif

        for (int index = 0; index < mainJumpScares.Length; index++)
        {
            MainJumpScareBase mainJumpScare = mainJumpScares[index];

            if (mainJumpScare == null)
            {
                continue;
            }

            RegisterMainJumpScare(mainJumpScare);
        }

        if (_enableDebugLog == true)
        {
            Debug.Log($"JumpScareManager 메인 점프스케어 등록 완료. Count : {_mainJumpScareById.Count}", this);
        }
    }

    public void ExecuteMainJumpScare(string id)
    {
        if (string.IsNullOrWhiteSpace(id) == true)
        {
            Debug.LogError("실행 요청된 메인 점프스케어 ID가 비어 있습니다.", this);
            return;
        }

        if (_mainJumpScareById.TryGetValue(id, out MainJumpScareBase mainJumpScare) == false)
        {
            Debug.LogError($"메인 점프스케어 ID [{id}] 를 찾을 수 없습니다.", this);
            return;
        }

        if (mainJumpScare.CanActive == false)
        {
            if (_enableDebugLog == true)
            {
                Debug.LogWarning($"메인 점프스케어 [{id}] 는 CanActive 가 false 이므로 실행되지 않습니다.", this);
            }

            return;
        }

        if (mainJumpScare.State == EMainJumpScareState.Playing)
        {
            if (_enableDebugLog == true)
            {
                Debug.LogWarning($"메인 점프스케어 [{id}] 는 이미 실행 중입니다.", this);
            }

            return;
        }

        if (mainJumpScare.State == EMainJumpScareState.Finished)
        {
            if (_enableDebugLog == true)
            {
                Debug.LogWarning($"메인 점프스케어 [{id}] 는 이미 종료된 상태입니다.", this);
            }

            return;
        }

        bool wasEmptyBeforeExecute = _playingMainJumpScareIds.Count == 0;

        _playingMainJumpScareIds.Add(id);

        if (wasEmptyBeforeExecute == true)
        {
            PauseSubJumpScare();
        }

        if (_enableDebugLog == true)
        {
            Debug.Log($"메인 점프스케어 실행 요청 : [{id}]", this);
        }

        mainJumpScare.Execute();
    }

    public void SetMainJumpScareCanActive(string id, bool canActive)
    {
        if (string.IsNullOrWhiteSpace(id) == true)
        {
            Debug.LogError("CanActive 변경 요청된 메인 점프스케어 ID가 비어 있습니다.", this);
            return;
        }

        if (_mainJumpScareById.TryGetValue(id, out MainJumpScareBase mainJumpScare) == false)
        {
            Debug.LogError($"CanActive 변경 대상 메인 점프스케어 ID [{id}] 를 찾을 수 없습니다.", this);
            return;
        }

        mainJumpScare.SetCanActive(canActive);

        if (_enableDebugLog == true)
        {
            Debug.Log($"메인 점프스케어 [{id}] CanActive 변경 : {canActive}", this);
        }
    }

    public void NotifyMainJumpScareFinished(string id)
    {
        if (string.IsNullOrWhiteSpace(id) == true)
        {
            Debug.LogError("종료 통지된 메인 점프스케어 ID가 비어 있습니다.", this);
            return;
        }

        if (_mainJumpScareById.ContainsKey(id) == false)
        {
            Debug.LogError($"종료 통지된 메인 점프스케어 ID [{id}] 는 등록되어 있지 않습니다.", this);
            return;
        }

        if (_playingMainJumpScareIds.Contains(id) == false)
        {
            Debug.LogWarning($"메인 점프스케어 [{id}] 종료 통지를 받았지만 실행 중 목록에 없습니다.", this);
        }
        else
        {
            _playingMainJumpScareIds.Remove(id);
        }

        if (_enableDebugLog == true)
        {
            Debug.Log($"메인 점프스케어 종료 통지 : [{id}], Remaining Playing Count : {_playingMainJumpScareIds.Count}", this);
        }

        if (_playingMainJumpScareIds.Count == 0)
        {
            ResumeSubJumpScare();
        }
    }

    private void RegisterMainJumpScare(MainJumpScareBase mainJumpScare)
    {
        if (mainJumpScare == null)
        {
            return;
        }

        string id = mainJumpScare.Id;

        if (string.IsNullOrWhiteSpace(id) == true)
        {
            Debug.LogError($"메인 점프스케어 [{mainJumpScare.name}] 의 ID가 비어 있어 등록할 수 없습니다.", mainJumpScare);
            return;
        }

        if (_mainJumpScareById.ContainsKey(id) == true)
        {
            Debug.LogError($"중복된 메인 점프스케어 ID [{id}] 가 발견되었습니다. [{mainJumpScare.name}] 는 등록되지 않습니다.", mainJumpScare);
            return;
        }

        _mainJumpScareById.Add(id, mainJumpScare);

        if (_enableDebugLog == true)
        {
            Debug.Log($"메인 점프스케어 등록 : [{id}] -> {mainJumpScare.name}", mainJumpScare);
        }
    }

    private void PauseSubJumpScare()
    {
        if (_enableDebugLog == true)
        {
            Debug.Log("서브 점프스케어 중단", this);
        }

        /*
        예시
        if (_subJumpScareManager != null)
        {
            _subJumpScareManager.Pause();
        }
        */
    }

    private void ResumeSubJumpScare()
    {
        if (_enableDebugLog == true)
        {
            Debug.Log("서브 점프스케어 재개", this);
        }

        /*
        예시
        if (_subJumpScareManager != null)
        {
            _subJumpScareManager.Resume();
        }
        */
    }

    [ContextMenu("Debug/Try Select Periodic")]
    public void TrySelectPeriodic()
    {
        SubJumpScareContext context = CreateContext();
        SubJumpScareSelectionResult selectedResult = _selectionCoordinator.SelectPeriodic(database, context);
        SubJumpScareSelectionResult finalResult = ResolvePeriodicSelectionResult(selectedResult);

        LogResult(context, finalResult);
        PublishRawResult(finalResult);
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

    private SubJumpScareSelectionResult ResolvePeriodicSelectionResult(SubJumpScareSelectionResult selectedResult)
    {
        if (selectedResult.IsSuccess == false)
        {
            return selectedResult;
        }

        if (selectedResult.Data == null)
        {
            return selectedResult;
        }

        if (selectedResult.Data.Type == ESubJumpScareType.PostProcess)
        {
            PostProcessSubJumpScareDefinitionSO definition;

            if (database == null || database.TryGetPostProcessDefinition(selectedResult.Data.Id, out definition) == false)
            {
                return SubJumpScareSelectionResult.CreateFail(
                    ESubJumpScareTriggerType.Periodic,
                    "선택된 포스트 프로세스 정의를 찾지 못했습니다.");
            }

            if (_postProcessExecutor == null)
            {
                return SubJumpScareSelectionResult.CreateFail(
                    ESubJumpScareTriggerType.Periodic,
                    "포스트 프로세스 실행기가 연결되지 않았습니다.");
            }

            bool isExecuted = _postProcessExecutor.TryExecute(definition);

            if (isExecuted == false)
            {
                return SubJumpScareSelectionResult.CreateFail(
                    ESubJumpScareTriggerType.Periodic,
                    "포스트 프로세스 실행에 실패했습니다.");
            }

            _selectionCoordinator.ConfirmPeriodicTriggered(selectedResult);
            return selectedResult;
        }

        if (selectedResult.Data.Type == ESubJumpScareType.Sound)
        {
            return SubJumpScareSelectionResult.CreateFail(
                ESubJumpScareTriggerType.Periodic,
                "사운드 점프스케어 실행기는 아직 연결되지 않았습니다.");
        }

        return SubJumpScareSelectionResult.CreateFail(
            ESubJumpScareTriggerType.Periodic,
            "주기 검사에서 지원하지 않는 타입이 선택되었습니다.");
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

    private void UpdateRuntimeStates()
    {
        if (_postProcessExecutor != null)
        {
            isPostProcessActive = _postProcessExecutor.IsPlaying;
        }
        else
        {
            isPostProcessActive = false;
        }
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