using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    private static AchievementManager _instance;
    public static AchievementManager Instance => _instance;
    
    [Header("Definition")]
    [SerializeField] private AchievementDefinitionDatabaseSO _definitionDatabase;

    [Header("Debug")]
    [SerializeField] private bool _logOnUnlock = true;

    private IAchievementDefinitionRepository _definitionRepository;
    private IAchievementStateRepository _stateRepository;
    private IAchievementRunStatisticsRepository _runStatisticsRepository;

    private readonly List<AchievementState> _states = new List<AchievementState>();
    private readonly Dictionary<string, AchievementState> _stateMap = new Dictionary<string, AchievementState>();

    private AchievementRunStatistics _runStatistics;

    public event Action<AchievementDefinition, AchievementState> AchievementUnlocked;
    public event Action<AchievementDefinition, AchievementState> AchievementStateChanged;
    public event Action AchievementListChanged;

    public IReadOnlyList<AchievementState> States => _states;
    public AchievementRunStatistics RunStatistics => _runStatistics;

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

    private void InitializeRepositories()
    {
        _definitionRepository = new AchievementDefinitionRepository(_definitionDatabase);
        _stateRepository = new AchievementStateRepository();
        _runStatisticsRepository = new AchievementRunStatisticsRepository();
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
    }

    public void HandleEvent(AchievementEvent achievementEvent)
    {
        /*_runStatistics.IncrementSonarUseCount();
        _runStatisticsRepository.Save(_runStatistics);*/

        AddProgressAndTryUnlock(achievementEvent.Key, 1);
    }
    
    public bool TryGetDefinition(string achievementId, out AchievementDefinition definition)
    {
        return _definitionRepository.TryGetDefinition(achievementId, out definition);
    }

    public bool TryGetState(string achievementId, out AchievementState state)
    {
        return _stateMap.TryGetValue(achievementId, out state);
    }

    public IReadOnlyList<AchievementDefinition> GetAllDefinitions()
    {
        return _definitionRepository.GetAllDefinitions();
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
        return _definitionRepository.GetAllDefinitions().Count;
    }

    public void ResetCurrentRunStatistics()
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

        bool isUnlockedNow = state.CurrentValue >= definition.TargetValue && state.TryUnlock() == true;

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
        }
    }

    private void ForceUnlock(string achievementId)
    {
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

        state.SetCurrentValue(definition.TargetValue);
        bool unlocked = state.TryUnlock();

        SaveStates();
        AchievementStateChanged?.Invoke(definition, state);
        AchievementListChanged?.Invoke();

        if (unlocked == true)
        {
            if (_logOnUnlock == true)
            {
                Debug.Log($"[AchievementManager] Unlock :: {definition.Title}");
            }

            AchievementUnlocked?.Invoke(definition, state);
        }
    }

    private void SaveStates()
    {
        _stateRepository.SaveStates(_states);
    }
}
