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
    
    private IAchievementStateRepository _stateRepository;

    private readonly List<AchievementState> _states = new List<AchievementState>();
    private readonly Dictionary<string, AchievementState> _stateMap = new Dictionary<string, AchievementState>();

    private CompositeSubscription _subscriptions;

    [Title("Debug Command")] 
    [FoldoutGroup("Command")] 
    [LabelText("대상 업적 ID")] 
    [SerializeField] private AchievementKeyReference _debugAchievementId;

    [FoldoutGroup("Command")]
    [LabelText("진행도 증가량")]
    [MinValue(1)]
    [SerializeField] private int _debugProgressAmount = 1;

    public event Action<AchievementDefinition, AchievementState> AchievementUnlocked;
    public event Action<AchievementDefinition, AchievementState> AchievementStateChanged;
    public event Action AchievementListChanged;

    public IReadOnlyList<AchievementState> States => _states;

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
        _stateRepository = new AchievementStatePlayerPrefsRepository();
    }

    private void EnsureRepositories()
    {
        if(_stateRepository != null)
        {
            return;
        }

        if (_definitionDatabase == null)
        {
            return;
        }

        InitializeRepositories();
    }

    private void LoadAllData()
    {
        EnsureRepositories();

        if (_definitionDatabase == null || _stateRepository == null)
        {
            _states.Clear();
            _stateMap.Clear();
            return;
        }

        IReadOnlyList<AchievementDefinition> definitions = _definitionDatabase.GetAllDefinitions();

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
        _subscriptions.Add(hub.Subscribe<StatisticsSonarUsedEvent>(_ => AchievementListChanged?.Invoke()));
        _subscriptions.Add(hub.Subscribe<StatisticsLidarRestoredEvent>(_ => AchievementListChanged?.Invoke()));
        _subscriptions.Add(hub.Subscribe<StatisticsAiQuestionEvent>(_ => AchievementListChanged?.Invoke()));
        _subscriptions.Add(hub.Subscribe<StatisticsLogCollectedEvent>(_ => AchievementListChanged?.Invoke()));
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

    private void HandleAchievementEvent(AchievementEvent achievementEvent)
    {
        if (IsStatisticDrivenAchievement(achievementEvent.AchievementId) == true)
        {
            TryUnlockAchievement(achievementEvent.AchievementId);
            return;
        }

        AddProgressAndTryUnlock(achievementEvent.AchievementId, 1);
    }

    public bool TryGetDefinition(string achievementId, out AchievementDefinition definition)
    {
        EnsureRepositories();

        definition = null;

        if (_definitionDatabase == null)
        {
            return false;
        }

        return _definitionDatabase.TryGetDefinition(achievementId, out definition);
    }

    public bool TryGetState(string achievementId, out AchievementState state)
    {
        return _stateMap.TryGetValue(achievementId, out state);
    }

    public IReadOnlyList<AchievementDefinition> GetAllDefinitions()
    {
        EnsureRepositories();

        if (_definitionDatabase == null)
        {
            return Array.Empty<AchievementDefinition>();
        }

        IReadOnlyList<AchievementDefinition> definitions = _definitionDatabase.GetAllDefinitions();
        return definitions ?? Array.Empty<AchievementDefinition>();
    }

    public int GetDisplayCurrentValue(AchievementDefinition definition, AchievementState state)
    {
        if (definition == null)
        {
            return 0;
        }

        if (TryGetStatisticProgressValue(definition.Id, out int progressValue) == true)
        {
            return Mathf.Min(progressValue, definition.TargetValue);
        }

        return state != null ? state.CurrentValue : 0;
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
        return GetAllDefinitions().Count;
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

        if (_definitionDatabase.TryGetDefinition(achievementId, out AchievementDefinition definition) == false)
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

    private void TryUnlockAchievement(string achievementId)
    {
        if (string.IsNullOrWhiteSpace(achievementId) == true)
        {
            return;
        }

        if (_definitionDatabase.TryGetDefinition(achievementId, out AchievementDefinition definition) == false)
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

        bool isUnlockedNow = state.TryUnlock();
        if (isUnlockedNow == false)
        {
            return;
        }

        SaveStates();
        AchievementStateChanged?.Invoke(definition, state);
        AchievementListChanged?.Invoke();

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

    private void ForceUnlockAchievement(string achievementId)
    {
        if (string.IsNullOrWhiteSpace(achievementId) == true)
        {
            return;
        }

        if (_definitionDatabase.TryGetDefinition(achievementId, out AchievementDefinition definition) == false)
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

        if (_definitionDatabase.TryGetDefinition(achievementId, out AchievementDefinition definition) == false)
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

        IReadOnlyList<AchievementDefinition> definitions = _definitionDatabase.GetAllDefinitions();

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

    private bool IsStatisticDrivenAchievement(string achievementId)
    {
        return achievementId == AchievementKey.Collection_FirstRecord
               || achievementId == AchievementKey.Collection_AllLogsComplete
               || achievementId == AchievementKey.Mechanic_FirstSignal
               || achievementId == AchievementKey.Mechanic_RestorationExpert
               || achievementId == AchievementKey.Mechanic_Chatterbox;
    }

    private bool TryGetStatisticProgressValue(string achievementId, out int progressValue)
    {
        progressValue = 0;

        StatisticsManager statisticsManager = StatisticsManager.Instance;
        if (statisticsManager == null)
        {
            return false;
        }

        switch (achievementId)
        {
            case AchievementKey.Collection_FirstRecord:
            case AchievementKey.Collection_AllLogsComplete:
                progressValue = statisticsManager.GetCollectedLogCount();
                return true;

            case AchievementKey.Mechanic_FirstSignal:
                progressValue = statisticsManager.GetTotalSonarUseCount();
                return true;

            case AchievementKey.Mechanic_RestorationExpert:
                progressValue = statisticsManager.GetTotalLidarRestoreCount();
                return true;

            case AchievementKey.Mechanic_Chatterbox:
                progressValue = statisticsManager.GetTotalAiQuestionCount();
                return true;

            default:
                return false;
        }
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
    [TableList(AlwaysExpanded = true)]
    [LabelText("업적 상태 목록")]
    public List<AchievementDebugStateView> DebugStateViews
    {
        get
        {
            List<AchievementDebugStateView> views = new List<AchievementDebugStateView>();

            IReadOnlyList<AchievementDefinition> definitions = GetAllDefinitions();

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
                    GetDisplayCurrentValue(definition, state),
                    definition.TargetValue,
                    state != null && state.IsUnlocked));
            }

            return views;
        }
    }
    [FoldoutGroup("Command")]
    [Button("데이터 세이브", ButtonSizes.Medium)]
    private void SaveStates()
    {
        _stateRepository.SaveStates(_states);
        Debug.Log("[AchievementManager] Reload All Data");
        
    }

    [FoldoutGroup("Command")]
    [Button("데이터 리로드", ButtonSizes.Medium)]
    private void DebugReloadAllData()
    {
        LoadAllData();
        AchievementListChanged?.Invoke();

        Debug.Log("[AchievementManager] Reload All Data");
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
