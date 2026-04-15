using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class AchievementDebugCommands
{
    private readonly AchievementProgressService _progressService;
    private readonly AchievementUnlockService _unlockService;
    private readonly Action _raiseListChanged;

    public AchievementDebugCommands(
        AchievementProgressService progressService,
        AchievementUnlockService unlockService,
        Action raiseListChanged)
    {
        _progressService = progressService;
        _unlockService = unlockService;
        _raiseListChanged = raiseListChanged;
    }

    public int GetTotalCount()
    {
        return _progressService != null ? _progressService.GetTotalCount() : 0;
    }

    public int GetUnlockedCount()
    {
        return _progressService != null ? _progressService.GetUnlockedCount() : 0;
    }

    public List<AchievementDebugStateView> GetDebugStateViews()
    {
        List<AchievementDebugStateView> views = new List<AchievementDebugStateView>();
        if (_progressService == null)
        {
            return views;
        }

        IReadOnlyList<AchievementDefinition> definitions = _progressService.GetAllDefinitions();
        for (int index = 0; index < definitions.Count; index++)
        {
            AchievementDefinition definition = definitions[index];
            if (definition == null)
            {
                continue;
            }

            _progressService.TryGetState(definition.Id, out AchievementState state);
            views.Add(new AchievementDebugStateView(
                definition.Id,
                definition.Title,
                _progressService.GetDisplayCurrentValue(definition, state),
                definition.TargetValue,
                state != null && state.IsUnlocked));
        }

        return views;
    }

    public void SaveStates()
    {
        _progressService?.SaveStates();
        Debug.Log("[AchievementManager] Save States");
    }

    public void ReloadAllData()
    {
        _progressService?.LoadAllData();
        _raiseListChanged?.Invoke();
        Debug.Log("[AchievementManager] Reload All Data");
    }

    public void ResetAllAchievementStates()
    {
        if (_progressService == null)
        {
            return;
        }

        IReadOnlyList<AchievementState> states = _progressService.States;
        for (int index = 0; index < states.Count; index++)
        {
            states[index].Reset();
        }

        _progressService.SaveStates();
        _raiseListChanged?.Invoke();
        Debug.Log("[AchievementManager] Reset All Achievement States");
    }

    public void AddProgressToTargetAchievement(string achievementId, int progressAmount)
    {
        if (string.IsNullOrWhiteSpace(achievementId))
        {
            Debug.LogWarning("[AchievementManager] Debug target achievement id is empty");
            return;
        }

        _unlockService?.AddProgressAndTryUnlock(achievementId, progressAmount);
    }

    public void ForceUnlockTargetAchievement(string achievementId)
    {
        if (string.IsNullOrWhiteSpace(achievementId))
        {
            Debug.LogWarning("[AchievementManager] Debug target achievement id is empty");
            return;
        }

        _unlockService?.ForceUnlockAchievement(achievementId);
    }

    public void ForceLockTargetAchievement(string achievementId)
    {
        if (string.IsNullOrWhiteSpace(achievementId))
        {
            Debug.LogWarning("[AchievementManager] Debug target achievement id is empty");
            return;
        }

        _unlockService?.ForceLockAchievement(achievementId);
    }
}
