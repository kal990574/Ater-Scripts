using System.Collections.Generic;
using UnityEngine;

public abstract class GroupedAchievementSubInterpreterBase : IAchievementSubInterpreter
{
    protected readonly GameEventPublisher Publisher = new GameEventPublisher();

    private readonly HashSet<string> _achievementIds = new HashSet<string>();
    private readonly HashSet<string> _pendingAchievementIds = new HashSet<string>();
    private CompositeSubscription _subscriptions;
    private bool _isEnabled;
    private AchievementManager _achievementManager;

    protected GroupedAchievementSubInterpreterBase(Object source, IEnumerable<string> achievementIds)
    {
        Publisher.SetSource(source);

        if (achievementIds == null)
        {
            return;
        }

        foreach (string achievementId in achievementIds)
        {
            if (string.IsNullOrWhiteSpace(achievementId) == true)
            {
                continue;
            }

            _achievementIds.Add(achievementId);
        }
    }

    public void Enable()
    {
        if (_isEnabled == true)
        {
            return;
        }

        _achievementManager = AchievementManager.Instance;
        RefreshPendingAchievements();
        if (_pendingAchievementIds.Count == 0)
        {
            return;
        }

        if (_achievementManager != null)
        {
            _achievementManager.AchievementUnlocked += OnAchievementUnlocked;
        }

        GameEventHub hub = GameEventHub.Instance;
        if (hub == null)
        {
            if (_achievementManager != null)
            {
                _achievementManager.AchievementUnlocked -= OnAchievementUnlocked;
                _achievementManager = null;
            }

            return;
        }

        _subscriptions = new CompositeSubscription();
        Subscribe(hub, _subscriptions);
        _isEnabled = true;
    }

    public void Disable()
    {
        if (_isEnabled == false)
        {
            return;
        }

        if (_achievementManager != null)
        {
            _achievementManager.AchievementUnlocked -= OnAchievementUnlocked;
            _achievementManager = null;
        }

        _subscriptions?.Dispose();
        _subscriptions = null;
        _isEnabled = false;
        Reset();
    }

    protected bool ShouldCheckAchievement(string achievementId)
    {
        return string.IsNullOrWhiteSpace(achievementId) == false && _pendingAchievementIds.Contains(achievementId) == true;
    }
    
    protected bool TryGetDefinition(string achievementId, out AchievementDefinition definition)
    {
        definition = null;

        AchievementManager manager = _achievementManager ?? AchievementManager.Instance;
        if (manager == null)
        {
            return false;
        }

        return manager.TryGetDefinition(achievementId, out definition);
    }

    protected void PublishAchievement(string achievementId)
    {
        if (ShouldCheckAchievement(achievementId) == false)
        {
            if (_pendingAchievementIds.Count == 0)
            {
                Disable();
            }

            return;
        }

        Publisher.TryPublish(context => new AchievementEvent(context, achievementId));
    }

    private void RefreshPendingAchievements()
    {
        _pendingAchievementIds.Clear();

        AchievementManager manager = _achievementManager ?? AchievementManager.Instance;
        if (manager == null)
        {
            foreach (string achievementId in _achievementIds)
            {
                _pendingAchievementIds.Add(achievementId);
            }

            return;
        }

        foreach (string achievementId in _achievementIds)
        {
            if (manager.TryGetState(achievementId, out AchievementState state) == true && state.IsUnlocked == true)
            {
                continue;
            }

            _pendingAchievementIds.Add(achievementId);
        }
    }

    private void OnAchievementUnlocked(AchievementDefinition definition, AchievementState state)
    {
        if (definition == null)
        {
            return;
        }

        _pendingAchievementIds.Remove(definition.Id);
        if (_pendingAchievementIds.Count == 0)
        {
            Disable();
        }
    }

    protected abstract void Subscribe(GameEventHub hub, CompositeSubscription subscriptions);

    protected virtual void Reset()
    {
    }
    
    protected void EvaluateThresholdAchievement(string achievementId, int progressValue)
    {
        if (ShouldCheckAchievement(achievementId) == false)
        {
            return;
        }

        if (TryGetDefinition(achievementId, out AchievementDefinition definition) == false)
        {
            return;
        }

        if (progressValue < definition.TargetValue)
        {
            return;
        }

        PublishAchievement(achievementId);
    }
}
