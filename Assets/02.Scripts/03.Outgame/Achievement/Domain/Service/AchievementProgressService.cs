using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementProgressService
{
    private readonly AchievementDefinitionDatabaseSO _definitionDatabase;
    private readonly IAchievementStateRepository _stateRepository;
    private readonly List<AchievementState> _states = new List<AchievementState>();
    private readonly Dictionary<string, AchievementState> _stateMap = new Dictionary<string, AchievementState>();

    private CompositeSubscription _subscriptions;

    public AchievementProgressService(
        AchievementDefinitionDatabaseSO definitionDatabase,
        IAchievementStateRepository stateRepository)
    {
        _definitionDatabase = definitionDatabase;
        _stateRepository = stateRepository;
    }

    public IReadOnlyList<AchievementState> States => _states;

    public void LoadAllData()
    {
        if (_definitionDatabase == null || _stateRepository == null)
        {
            _states.Clear();
            _stateMap.Clear();
            return;
        }

        IReadOnlyList<AchievementDefinition> definitions = _definitionDatabase.GetAllDefinitions();
        List<AchievementState> loadedStates = _stateRepository.LoadStates(definitions);

        _states.Clear();
        _stateMap.Clear();

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

    public void SaveStates()
    {
        if (_stateRepository == null)
        {
            return;
        }

        _stateRepository.SaveStates(_states);
    }

    public void SubscribeEvents(AchievementUnlockService unlockService, Action onStatisticChanged)
    {
        DisposeSubscriptions();

        GameEventHub hub = GameEventHub.Instance;
        if (hub == null || unlockService == null)
        {
            return;
        }

        _subscriptions = new CompositeSubscription();
        _subscriptions.Add(hub.Subscribe<AchievementEvent>(achievementEvent =>
        {
            if (unlockService.IsStatisticDrivenAchievement(achievementEvent.AchievementId))
            {
                unlockService.TryUnlockAchievement(achievementEvent.AchievementId);
                return;
            }

            unlockService.AddProgressAndTryUnlock(achievementEvent.AchievementId, 1);
        }));
        _subscriptions.Add(hub.Subscribe<StatisticsSonarUsedEvent>(_ => onStatisticChanged?.Invoke()));
        _subscriptions.Add(hub.Subscribe<StatisticsLidarRestoredEvent>(_ => onStatisticChanged?.Invoke()));
        _subscriptions.Add(hub.Subscribe<StatisticsAiQuestionEvent>(_ => onStatisticChanged?.Invoke()));
        _subscriptions.Add(hub.Subscribe<StatisticsLogCollectedEvent>(_ => onStatisticChanged?.Invoke()));
    }

    public void DisposeSubscriptions()
    {
        if (_subscriptions == null)
        {
            return;
        }

        _subscriptions.Dispose();
        _subscriptions = null;
    }

    public bool TryGetDefinition(string achievementId, out AchievementDefinition definition)
    {
        definition = null;
        return _definitionDatabase != null
               && _definitionDatabase.TryGetDefinition(achievementId, out definition);
    }

    public bool TryGetState(string achievementId, out AchievementState state)
    {
        return _stateMap.TryGetValue(achievementId, out state);
    }

    public IReadOnlyList<AchievementDefinition> GetAllDefinitions()
    {
        if (_definitionDatabase == null)
        {
            return Array.Empty<AchievementDefinition>();
        }

        return _definitionDatabase.GetAllDefinitions() ?? Array.Empty<AchievementDefinition>();
    }

    public int GetDisplayCurrentValue(AchievementDefinition definition, AchievementState state)
    {
        if (definition == null)
        {
            return 0;
        }

        if (TryGetStatisticProgressValue(definition.Id, out int progressValue))
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
            if (_states[index].IsUnlocked)
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
}
