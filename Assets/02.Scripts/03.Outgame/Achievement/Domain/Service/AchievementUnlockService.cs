using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementUnlockService
{
    private readonly AchievementDefinitionDatabaseSO _definitionDatabase;
    private readonly AchievementProgressService _progressService;
    private readonly Func<bool> _shouldLogUnlock;
    private readonly Action<AchievementDefinition, AchievementState> _onStateChanged;
    private readonly Action<AchievementDefinition, AchievementState> _onUnlocked;

    public AchievementUnlockService(
        AchievementDefinitionDatabaseSO definitionDatabase,
        AchievementProgressService progressService,
        Func<bool> shouldLogUnlock,
        Action<AchievementDefinition, AchievementState> onStateChanged,
        Action<AchievementDefinition, AchievementState> onUnlocked)
    {
        _definitionDatabase = definitionDatabase;
        _progressService = progressService;
        _shouldLogUnlock = shouldLogUnlock;
        _onStateChanged = onStateChanged;
        _onUnlocked = onUnlocked;
    }

    public void AddProgressAndTryUnlock(string achievementId, int amount)
    {
        if (string.IsNullOrWhiteSpace(achievementId) || amount <= 0)
        {
            return;
        }

        if (!TryResolveTarget(achievementId, out AchievementDefinition definition, out AchievementState state))
        {
            return;
        }

        if (state.IsUnlocked)
        {
            return;
        }

        state.AddProgress(amount);

        bool isUnlockedNow = false;
        if (state.CurrentValue >= definition.TargetValue)
        {
            isUnlockedNow = state.TryUnlock();
        }

        NotifyStateChanged(definition, state);

        if (isUnlockedNow)
        {
            NotifyUnlocked(definition, state, "[AchievementManager] Unlock");
            TryUnlockAterMaster(achievementId);
        }
    }

    public void TryUnlockAchievement(string achievementId)
    {
        if (string.IsNullOrWhiteSpace(achievementId))
        {
            return;
        }

        if (!TryResolveTarget(achievementId, out AchievementDefinition definition, out AchievementState state))
        {
            return;
        }

        if (state.IsUnlocked)
        {
            return;
        }

        if (!state.TryUnlock())
        {
            return;
        }

        NotifyStateChanged(definition, state);
        NotifyUnlocked(definition, state, "[AchievementManager] Unlock");
        TryUnlockAterMaster(achievementId);
    }

    public void ForceUnlockAchievement(string achievementId)
    {
        if (string.IsNullOrWhiteSpace(achievementId))
        {
            return;
        }

        if (!TryResolveTarget(achievementId, out AchievementDefinition definition, out AchievementState state, true))
        {
            return;
        }

        if (state.IsUnlocked)
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

        NotifyStateChanged(definition, state);

        if (isUnlockedNow)
        {
            NotifyUnlocked(definition, state, "[AchievementManager] Force Unlock");
            TryUnlockAterMaster(achievementId);
        }
    }

    public void ForceLockAchievement(string achievementId)
    {
        if (string.IsNullOrWhiteSpace(achievementId))
        {
            return;
        }

        if (!TryResolveTarget(achievementId, out AchievementDefinition definition, out AchievementState state, true))
        {
            return;
        }

        state.Reset();
        NotifyStateChanged(definition, state);
        Debug.Log($"[AchievementManager] Force Lock :: {achievementId}");
    }

    public bool IsStatisticDrivenAchievement(string achievementId)
    {
        return achievementId == AchievementKey.Collection_FirstRecord
               || achievementId == AchievementKey.Collection_AllLogsComplete
               || achievementId == AchievementKey.Mechanic_FirstSignal
               || achievementId == AchievementKey.Mechanic_RestorationExpert
               || achievementId == AchievementKey.Mechanic_Chatterbox;
    }

    private void TryUnlockAterMaster(string sourceAchievementId)
    {
        if (sourceAchievementId == AchievementKey.Meta_AterMaster)
        {
            return;
        }

        if (!_progressService.TryGetState(AchievementKey.Meta_AterMaster, out AchievementState masterState) || masterState.IsUnlocked)
        {
            return;
        }

        IReadOnlyList<AchievementDefinition> definitions = _progressService.GetAllDefinitions();
        for (int index = 0; index < definitions.Count; index++)
        {
            AchievementDefinition definition = definitions[index];
            if (definition == null || definition.Id == AchievementKey.Meta_AterMaster)
            {
                continue;
            }

            if (!_progressService.TryGetState(definition.Id, out AchievementState state) || !state.IsUnlocked)
            {
                return;
            }
        }

        AddProgressAndTryUnlock(AchievementKey.Meta_AterMaster, 1);
    }

    private void NotifyStateChanged(AchievementDefinition definition, AchievementState state)
    {
        _progressService.SaveStates();
        _onStateChanged?.Invoke(definition, state);
    }

    private void NotifyUnlocked(AchievementDefinition definition, AchievementState state, string logPrefix)
    {
        if (_shouldLogUnlock != null && _shouldLogUnlock())
        {
            Debug.Log($"{logPrefix} :: {definition.Title}");
        }

        _onUnlocked?.Invoke(definition, state);
    }

    private bool TryResolveTarget(
        string achievementId,
        out AchievementDefinition definition,
        out AchievementState state,
        bool logFailure = false)
    {
        definition = null;
        state = null;

        if (_definitionDatabase == null)
        {
            return false;
        }

        if (!_definitionDatabase.TryGetDefinition(achievementId, out definition))
        {
            if (logFailure)
            {
                Debug.LogWarning($"[AchievementManager] Invalid achievement id :: {achievementId}");
            }

            return false;
        }

        if (!_progressService.TryGetState(achievementId, out state))
        {
            if (logFailure)
            {
                Debug.LogWarning($"[AchievementManager] Missing state :: {achievementId}");
            }

            return false;
        }

        return true;
    }
}
