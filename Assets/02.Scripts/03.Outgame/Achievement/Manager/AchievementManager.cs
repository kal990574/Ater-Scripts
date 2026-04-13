using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    private static AchievementManager _instance;
    public static AchievementManager Instance => _instance;

    [Title("Definition")]
    [SerializeField] private AchievementDefinitionDatabaseSO _definitionDatabase;

    [Title("Debug Option")]
    [SerializeField] private bool _logOnUnlock = true;

    private IAchievementDefinitionRepository _definitionRepository;
    private IAchievementStateRepository _stateRepository;
    private IAchievementRunStatisticsRepository _runStatisticsRepository;
    private IAchievementCollectedLogRepository _collectedLogRepository;

    private readonly List<AchievementState> _states = new List<AchievementState>();
    private readonly Dictionary<string, AchievementState> _stateMap = new Dictionary<string, AchievementState>();

    private AchievementRunStatistics _runStatistics;
    private AchievementCurrentRunStatistics _currentRun;
    private AchievementCollectedLogState _collectedLogState;
    private CompositeSubscription _subscriptions;

    [Title("Runtime Debug")]
    [FoldoutGroup("Command")]
    [LabelText("대상 업적 ID")]
    [SerializeField] private AchievementKeyReference _debugAchievementId ;

    [FoldoutGroup("Command")]
    [LabelText("진행도 증가량")]
    [MinValue(1)]
    [SerializeField] private int _debugProgressAmount = 1;

    public event Action<AchievementDefinition, AchievementState> AchievementUnlocked;
    public event Action<AchievementDefinition, AchievementState> AchievementStateChanged;
    public event Action AchievementListChanged;

    public IReadOnlyList<AchievementState> States => _states;
    public AchievementRunStatistics RunStatistics => _runStatistics;
    public AchievementCurrentRunStatistics CurrentRun => _currentRun;
    public AchievementCollectedLogState CollectedLogState => _collectedLogState;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        InitializeRepositories();
        LoadAllData();
        BeginRun();
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        DisposeSubscriptions();
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }

        DisposeSubscriptions();
    }

    private void InitializeRepositories()
    {
        _definitionRepository = new AchievementDefinitionRepository(_definitionDatabase);
        _stateRepository = new AchievementStateRepository();
        _runStatisticsRepository = new AchievementRunStatisticsRepository();
        _collectedLogRepository = new AchievementCollectedLogRepository();
    }

    private void LoadAllData()
    {
        IReadOnlyList<AchievementDefinition> definitions = _definitionRepository.GetAllDefinitions();

        _states.Clear();
        _stateMap.Clear();

        List<AchievementState> loadedStates = _stateRepository.LoadStates(definitions);

        for (int index = 0; index < loadedStates.Count; index++)
        {
            AchievementState state = loadedStates[index];

            if (state == null)
            {
                continue;
            }

            _states.Add(state);

            if (_stateMap.ContainsKey(state.Id) == false)
            {
                _stateMap.Add(state.Id, state);
            }
        }

        _runStatistics = _runStatisticsRepository.Load();
        _collectedLogState = _collectedLogRepository.Load();
    }

    private void SubscribeEvents()
    {
        DisposeSubscriptions();

        GameEventHub hub = GameEventHub.Instance;
        if (hub == null)
        {
            return;
        }

        _subscriptions = new CompositeSubscription();
        _subscriptions.Add(hub.Subscribe<AchievementEvent>(HandleAchievementEvent));
        _subscriptions.Add(hub.Subscribe<AchievementRunEndedRawEvent>(HandleRunEnded));
        _subscriptions.Add(hub.Subscribe<LogCollectedRawEvent>(HandleLogCollected));
    }

    private void DisposeSubscriptions()
    {
        if (_subscriptions == null)
        {
            return;
        }

        _subscriptions.Dispose();
        _subscriptions = null;
    }

    public void BeginRun()
    {
        if (_currentRun == null)
        {
            _currentRun = new AchievementCurrentRunStatistics();
        }

        _currentRun.Begin();
    }

    private void HandleAchievementEvent(AchievementEvent achievementEvent)
    {
        AddProgressAndTryUnlock(achievementEvent.AchievementId, 1);
    }

    private void HandleRunEnded(AchievementRunEndedRawEvent rawEvent)
    {
        _runStatistics.AddSonarUseCount(rawEvent.Summary.SonarUseCount);
        _runStatistics.AddLidarRestoreCount(rawEvent.Summary.LidarRestoreCount);
        _runStatistics.AddAiQuestionCount(rawEvent.Summary.AiQuestionCount);

        _runStatisticsRepository.Save(_runStatistics);

        BeginRun();
    }

    private void HandleLogCollected(LogCollectedRawEvent rawEvent)
    {
        if (_collectedLogState == null)
        {
            _collectedLogState = new AchievementCollectedLogState();
        }

        bool added = _collectedLogState.TryCollect(rawEvent.LogId, rawEvent.IsTextLog);

        if (added == true)
        {
            _collectedLogRepository.Save(_collectedLogState);
        }
    }

    public bool TryGetDefinition(string achievementId, out AchievementDefinition definition)
    {
        definition = null;

        if (_definitionRepository == null)
        {
            return false;
        }

        return _definitionRepository.TryGetDefinition(achievementId, out definition);
    }

    public bool TryGetState(string achievementId, out AchievementState state)
    {
        return _stateMap.TryGetValue(achievementId, out state);
    }

    public IReadOnlyList<AchievementDefinition> GetAllDefinitions()
    {
        if (_definitionRepository == null)
        {
            return Array.Empty<AchievementDefinition>();
        }

        IReadOnlyList<AchievementDefinition> definitions = _definitionRepository.GetAllDefinitions();

        if (definitions == null)
        {
            return Array.Empty<AchievementDefinition>();
        }

        return definitions;
    }

    public bool HasCollectedLog(string logId)
    {
        if (_collectedLogState == null)
        {
            return false;
        }

        return _collectedLogState.HasCollected(logId);
    }

    public int GetCollectedLogCount()
    {
        if (_collectedLogState == null)
        {
            return 0;
        }

        return _collectedLogState.AllCollectedCount;
    }

    public int GetCollectedTextLogCount()
    {
        if (_collectedLogState == null)
        {
            return 0;
        }

        return _collectedLogState.TextCollectedCount;
    }

    public int GetUnlockedCount()
    {
        int count = 0;

        for (int index = 0; index < _states.Count; index++)
        {
            if (_states[index].IsUnlocked == true)
            {
                count++;
            }
        }

        return count;
    }

    public int GetTotalCount()
    {
        if (_definitionRepository == null)
        {
            return 0;
        }

        IReadOnlyList<AchievementDefinition> definitions = _definitionRepository.GetAllDefinitions();

        if (definitions == null)
        {
            return 0;
        }

        return definitions.Count;
    }

    public void ResetPersistentStatistics()
    {
        if (_runStatistics == null)
        {
            _runStatistics = new AchievementRunStatistics();
        }

        _runStatistics.ResetAll();
        _runStatisticsRepository.Save(_runStatistics);
    }

    private void AddProgressAndTryUnlock(string achievementId, int amount)
    {
        if (string.IsNullOrWhiteSpace(achievementId) == true)
        {
            return;
        }

        if (amount <= 0)
        {
            return;
        }

        if (_definitionRepository.TryGetDefinition(achievementId, out AchievementDefinition definition) == false)
        {
            return;
        }

        if (_stateMap.TryGetValue(achievementId, out AchievementState state) == false)
        {
            return;
        }

        if (state.IsUnlocked == true)
        {
            return;
        }

        state.AddProgress(amount);

        bool isUnlockedNow = false;

        if (state.CurrentValue >= definition.TargetValue)
        {
            isUnlockedNow = state.TryUnlock();
        }

        SaveStates();
        AchievementStateChanged?.Invoke(definition, state);
        AchievementListChanged?.Invoke();

        if (isUnlockedNow == true)
        {
            if (_logOnUnlock == true)
            {
                Debug.Log($"[AchievementManager] Unlock :: {definition.Title}");
            }

            AchievementUnlocked?.Invoke(definition, state);

            if (achievementId != AchievementKey.Meta_AterMaster)
            {
                TryUnlockAterMaster();
            }
        }
    }

    private void ForceUnlockAchievement(string achievementId)
    {
        if (string.IsNullOrWhiteSpace(achievementId) == true)
        {
            return;
        }

        if (_definitionRepository.TryGetDefinition(achievementId, out AchievementDefinition definition) == false)
        {
            Debug.LogWarning($"[AchievementManager] Invalid achievement id :: {achievementId}");
            return;
        }

        if (_stateMap.TryGetValue(achievementId, out AchievementState state) == false)
        {
            Debug.LogWarning($"[AchievementManager] Missing state :: {achievementId}");
            return;
        }

        if (state.IsUnlocked == true)
        {
            Debug.Log($"[AchievementManager] Already unlocked :: {achievementId}");
            return;
        }

        int requiredAmount = Mathf.Max(0, definition.TargetValue - state.CurrentValue);

        if (requiredAmount > 0)
        {
            state.AddProgress(requiredAmount);
        }

        bool isUnlockedNow = state.TryUnlock();

        SaveStates();
        AchievementStateChanged?.Invoke(definition, state);
        AchievementListChanged?.Invoke();

        if (isUnlockedNow == true)
        {
            if (_logOnUnlock == true)
            {
                Debug.Log($"[AchievementManager] Force Unlock :: {definition.Title}");
            }

            AchievementUnlocked?.Invoke(definition, state);

            if (achievementId != AchievementKey.Meta_AterMaster)
            {
                TryUnlockAterMaster();
            }
        }
    }

    private void ForceLockAchievement(string achievementId)
    {
        if (string.IsNullOrWhiteSpace(achievementId) == true)
        {
            return;
        }

        if (_definitionRepository.TryGetDefinition(achievementId, out AchievementDefinition definition) == false)
        {
            Debug.LogWarning($"[AchievementManager] Invalid achievement id :: {achievementId}");
            return;
        }

        if (_stateMap.TryGetValue(achievementId, out AchievementState state) == false)
        {
            Debug.LogWarning($"[AchievementManager] Missing state :: {achievementId}");
            return;
        }

        state.Reset();

        SaveStates();
        AchievementStateChanged?.Invoke(definition, state);
        AchievementListChanged?.Invoke();

        Debug.Log($"[AchievementManager] Force Lock :: {achievementId}");
    }

    private void TryUnlockAterMaster()
    {
        if (_stateMap.TryGetValue(AchievementKey.Meta_AterMaster, out AchievementState masterState) == false)
        {
            return;
        }

        if (masterState.IsUnlocked == true)
        {
            return;
        }

        IReadOnlyList<AchievementDefinition> definitions = _definitionRepository.GetAllDefinitions();

        for (int index = 0; index < definitions.Count; index++)
        {
            AchievementDefinition definition = definitions[index];

            if (definition == null)
            {
                continue;
            }

            if (definition.Id == AchievementKey.Meta_AterMaster)
            {
                continue;
            }

            if (_stateMap.TryGetValue(definition.Id, out AchievementState state) == false)
            {
                return;
            }

            if (state.IsUnlocked == false)
            {
                return;
            }
        }

        AddProgressAndTryUnlock(AchievementKey.Meta_AterMaster, 1);
    }

    private void SaveStates()
    {
        _stateRepository.SaveStates(_states);
    }

    [Title("Debug View")]
    [FoldoutGroup("View")]
    [ShowInInspector]
    [ReadOnly]
    [LabelText("총 업적 수")]
    public int DebugTotalCount => GetTotalCount();

    [FoldoutGroup("View")]
    [ShowInInspector]
    [ReadOnly]
    [LabelText("해금된 업적 수")]
    public int DebugUnlockedCount => GetUnlockedCount();

    [FoldoutGroup("View")]
    [ShowInInspector]
    [ReadOnly]
    [LabelText("소나 누적 횟수")]
    public int DebugPersistentSonarCount => _runStatistics != null ? _runStatistics.SonarUseCount : 0;

    [FoldoutGroup("View")]
    [ShowInInspector]
    [ReadOnly]
    [LabelText("라이더 누적 횟수")]
    public int DebugPersistentLidarCount => _runStatistics != null ? _runStatistics.LidarRestoreCount : 0;

    [FoldoutGroup("View")]
    [ShowInInspector]
    [ReadOnly]
    [LabelText("AI 누적 횟수")]
    public int DebugPersistentAiCount => _runStatistics != null ? _runStatistics.AiQuestionCount : 0;

    [FoldoutGroup("View")]
    [ShowInInspector]
    [ReadOnly]
    [LabelText("현재 런 소나 횟수")]
    public int DebugCurrentRunSonarCount => _currentRun != null ? _currentRun.SonarUseCount : 0;

    [FoldoutGroup("View")]
    [ShowInInspector]
    [ReadOnly]
    [LabelText("현재 런 라이더 횟수")]
    public int DebugCurrentRunLidarCount => _currentRun != null ? _currentRun.LidarRestoreCount : 0;

    [FoldoutGroup("View")]
    [ShowInInspector]
    [ReadOnly]
    [LabelText("현재 런 AI 횟수")]
    public int DebugCurrentRunAiCount => _currentRun != null ? _currentRun.AiQuestionCount : 0;

    [FoldoutGroup("View")]
    [ShowInInspector]
    [ReadOnly]
    [LabelText("수집 로그 총 개수")]
    public int DebugCollectedLogCount => GetCollectedLogCount();

    [FoldoutGroup("View")]
    [ShowInInspector]
    [ReadOnly]
    [LabelText("수집 텍스트 로그 개수")]
    public int DebugCollectedTextLogCount => GetCollectedTextLogCount();

    [FoldoutGroup("View")]
    [ShowInInspector]
    [ReadOnly]
    [TableList(AlwaysExpanded = true)]
    [LabelText("업적 상태 목록")]
    public List<AchievementDebugStateView> DebugStateViews
    {
        get
        {
            List<AchievementDebugStateView> views = new List<AchievementDebugStateView>();

            if (_definitionRepository == null)
            {
                return views;
            }

            IReadOnlyList<AchievementDefinition> definitions = _definitionRepository.GetAllDefinitions();

            if (definitions == null)
            {
                return views;
            }

            for (int index = 0; index < definitions.Count; index++)
            {
                AchievementDefinition definition = definitions[index];

                if (definition == null)
                {
                    continue;
                }

                AchievementState state = null;
                _stateMap.TryGetValue(definition.Id, out state);

                views.Add(new AchievementDebugStateView(
                    definition.Id,
                    definition.Title,
                    state != null ? state.CurrentValue : 0,
                    definition.TargetValue,
                    state != null && state.IsUnlocked));
            }

            return views;
        }
    }

    [Title("Debug Command")]
    [FoldoutGroup("Command")]
    [Button("데이터 리로드", ButtonSizes.Medium)]
    private void DebugReloadAllData()
    {
        LoadAllData();
        BeginRun();
        AchievementListChanged?.Invoke();

        Debug.Log("[AchievementManager] Reload All Data");
    }

    [FoldoutGroup("Command")]
    [Button("현재 런 초기화", ButtonSizes.Medium)]
    private void DebugResetCurrentRun()
    {
        BeginRun();
        Debug.Log("[AchievementManager] Reset Current Run");
    }

    [FoldoutGroup("Command")]
    [Button("누적 통계 초기화", ButtonSizes.Medium)]
    private void DebugResetPersistentStatistics()
    {
        ResetPersistentStatistics();
        Debug.Log("[AchievementManager] Reset Persistent Statistics");
    }

    [FoldoutGroup("Command")]
    [Button("수집 로그 초기화", ButtonSizes.Medium)]
    private void DebugResetCollectedLogs()
    {
        _collectedLogRepository.Reset();
        _collectedLogState = new AchievementCollectedLogState();

        Debug.Log("[AchievementManager] Reset Collected Logs");
    }

    [FoldoutGroup("Command")]
    [Button("업적 상태 전체 초기화", ButtonSizes.Large)]
    private void DebugResetAllAchievementStates()
    {
        for (int index = 0; index < _states.Count; index++)
        {
            _states[index].Reset();
        }

        SaveStates();
        AchievementListChanged?.Invoke();

        Debug.Log("[AchievementManager] Reset All Achievement States");
    }

    [FoldoutGroup("Command")]
    [Button("모든 업적 초기화", ButtonSizes.Large)]
    private void DebugResetEverything()
    {
        for (int index = 0; index < _states.Count; index++)
        {
            _states[index].Reset();
        }

        SaveStates();

        if (_runStatistics == null)
        {
            _runStatistics = new AchievementRunStatistics();
        }

        _runStatistics.ResetAll();
        _runStatisticsRepository.Save(_runStatistics);

        _collectedLogRepository.Reset();
        _collectedLogState = new AchievementCollectedLogState();

        BeginRun();
        AchievementListChanged?.Invoke();

        Debug.Log("[AchievementManager] Reset Everything");
    }
    [FoldoutGroup("Command")]
    [Button("대상 업적 진행도 증가", ButtonSizes.Medium)]
    private void DebugAddProgressToTargetAchievement()
    {
        if (string.IsNullOrWhiteSpace(_debugAchievementId) == true)
        {
            Debug.LogWarning("[AchievementManager] Debug target achievement id is empty");
            return;
        }

        AddProgressAndTryUnlock(_debugAchievementId, _debugProgressAmount);
    }

    [FoldoutGroup("Command")]
    [Button("대상 업적 강제 해금", ButtonSizes.Medium)]
    private void DebugForceUnlockTargetAchievement()
    {
        if (string.IsNullOrWhiteSpace(_debugAchievementId) == true)
        {
            Debug.LogWarning("[AchievementManager] Debug target achievement id is empty");
            return;
        }

        ForceUnlockAchievement(_debugAchievementId);
    }

    [FoldoutGroup("Command")]
    [Button("대상 업적 잠금 복구", ButtonSizes.Medium)]
    private void DebugForceLockTargetAchievement()
    {
        if (string.IsNullOrWhiteSpace(_debugAchievementId) == true)
        {
            Debug.LogWarning("[AchievementManager] Debug target achievement id is empty");
            return;
        }

        ForceLockAchievement(_debugAchievementId);
    }

    [Serializable]
    public class AchievementDebugStateView
    {
        [TableColumnWidth(220, false)]
        [LabelText("ID")]
        public string Id;

        [TableColumnWidth(180, false)]
        [LabelText("제목")]
        public string Title;

        [TableColumnWidth(80, false)]
        [LabelText("현재")]
        public int CurrentValue;

        [TableColumnWidth(80, false)]
        [LabelText("목표")]
        public int TargetValue;

        [TableColumnWidth(80, false)]
        [LabelText("해금")]
        public bool IsUnlocked;

        public AchievementDebugStateView(
            string id,
            string title,
            int currentValue,
            int targetValue,
            bool isUnlocked)
        {
            Id = id;
            Title = title;
            CurrentValue = currentValue;
            TargetValue = targetValue;
            IsUnlocked = isUnlocked;
        }
    }
}