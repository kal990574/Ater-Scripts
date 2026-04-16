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

    [Title("Debug Command")]
    [FoldoutGroup("Command")]
    [LabelText("Target Achievement ID")]
    [SerializeField] private AchievementKeyReference _debugAchievementId;

    [FoldoutGroup("Command")]
    [LabelText("Progress Amount")]
    [MinValue(1)]
    [SerializeField] private int _debugProgressAmount = 1;

    private AchievementProgressService _progressService;
    private AchievementUnlockService _unlockService;
    private AchievementDebugCommands _debugCommands;

    public event Action<AchievementDefinition, AchievementState> AchievementUnlocked;
    public event Action<AchievementDefinition, AchievementState> AchievementStateChanged;
    public event Action AchievementListChanged;

    public IReadOnlyList<AchievementState> States => _progressService != null
        ? _progressService.States
        : Array.Empty<AchievementState>();

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

        InitializeServices();
        _progressService.LoadAllData();
    }

    private void OnEnable()
    {
        _progressService?.SubscribeEvents(_unlockService, RaiseAchievementListChanged);
    }

    private void OnDisable()
    {
        _progressService?.DisposeSubscriptions();
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }

        _progressService?.DisposeSubscriptions();
    }

    public bool TryGetDefinition(string achievementId, out AchievementDefinition definition)
    {
        definition = null;
        return _progressService != null && _progressService.TryGetDefinition(achievementId, out definition);
    }

    public bool TryGetState(string achievementId, out AchievementState state)
    {
        state = null;
        return _progressService != null && _progressService.TryGetState(achievementId, out state);
    }

    public IReadOnlyList<AchievementDefinition> GetAllDefinitions()
    {
        return _progressService != null
            ? _progressService.GetAllDefinitions()
            : Array.Empty<AchievementDefinition>();
    }

    public int GetDisplayCurrentValue(AchievementDefinition definition, AchievementState state)
    {
        return _progressService != null
            ? _progressService.GetDisplayCurrentValue(definition, state)
            : 0;
    }

    public int GetUnlockedCount()
    {
        return _progressService != null
            ? _progressService.GetUnlockedCount()
            : 0;
    }

    public int GetTotalCount()
    {
        return _progressService != null
            ? _progressService.GetTotalCount()
            : 0;
    }

    [Title("Debug View")]
    [FoldoutGroup("View")]
    [ShowInInspector]
    [ReadOnly]
    [LabelText("Total Count")]
    public int DebugTotalCount => _debugCommands != null ? _debugCommands.GetTotalCount() : 0;

    [FoldoutGroup("View")]
    [ShowInInspector]
    [ReadOnly]
    [LabelText("Unlocked Count")]
    public int DebugUnlockedCount => _debugCommands != null ? _debugCommands.GetUnlockedCount() : 0;

    [FoldoutGroup("View")]
    [ShowInInspector]
    [ReadOnly]
    [TableList(AlwaysExpanded = true)]
    [LabelText("Achievement States")]
    public List<AchievementDebugStateView> DebugStateViews => _debugCommands != null
        ? _debugCommands.GetDebugStateViews()
        : new List<AchievementDebugStateView>();

    [FoldoutGroup("Command")]
    [Button("Save States", ButtonSizes.Medium)]
    private void SaveStates()
    {
        _debugCommands?.SaveStates();
    }

    [FoldoutGroup("Command")]
    [Button("Reload All Data", ButtonSizes.Medium)]
    private void ReloadAllData()
    {
        _debugCommands?.ReloadAllData();
    }

    [FoldoutGroup("Command")]
    [Button("Reset All Achievement States", ButtonSizes.Large)]
    private void ResetAllAchievementStates()
    {
        _debugCommands?.ResetAllAchievementStates();
    }

    [FoldoutGroup("Command")]
    [Button("Add Progress To Target", ButtonSizes.Medium)]
    private void AddProgressToTargetAchievement()
    {
        _debugCommands?.AddProgressToTargetAchievement(_debugAchievementId, _debugProgressAmount);
    }

    [FoldoutGroup("Command")]
    [Button("Force Unlock Target", ButtonSizes.Medium)]
    private void ForceUnlockTargetAchievement()
    {
        _debugCommands?.ForceUnlockTargetAchievement(_debugAchievementId);
    }

    [FoldoutGroup("Command")]
    [Button("Force Lock Target", ButtonSizes.Medium)]
    private void ForceLockTargetAchievement()
    {
        _debugCommands?.ForceLockTargetAchievement(_debugAchievementId);
    }

    private void InitializeServices()
    {
        IAchievementStateRepository repository = new AchievementStatePlayerPrefsRepository();
        _progressService = new AchievementProgressService(_definitionDatabase, repository);
        _unlockService = new AchievementUnlockService(
            _definitionDatabase,
            _progressService,
            () => _logOnUnlock,
            HandleAchievementStateChanged,
            HandleAchievementUnlocked);
        _debugCommands = new AchievementDebugCommands(_progressService, _unlockService, RaiseAchievementListChanged);
    }

    private void HandleAchievementStateChanged(AchievementDefinition definition, AchievementState state)
    {
        AchievementStateChanged?.Invoke(definition, state);
        RaiseAchievementListChanged();
    }

    private void HandleAchievementUnlocked(AchievementDefinition definition, AchievementState state)
    {
        AchievementUnlocked?.Invoke(definition, state);
    }

    private void RaiseAchievementListChanged()
    {
        AchievementListChanged?.Invoke();
    }
}
