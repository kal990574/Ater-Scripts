using UnityEngine;

public abstract class AchievementSubInterpreterBase : IAchievementSubInterpreter
{
    protected readonly GameEventPublisher Publisher = new GameEventPublisher();

    private readonly string _achievementId;
    private CompositeSubscription _subscriptions;
    private bool _isEnabled;
    private AchievementManager _achievementManager;

    protected AchievementSubInterpreterBase(Object source, string achievementId)
    {
        Publisher.SetSource(source);
        _achievementId = string.IsNullOrWhiteSpace(achievementId) ? AchievementKey.None : achievementId;
    }

    public void Enable()
    {
        if (_isEnabled == true)
        {
            return;
        }

        _achievementManager = AchievementManager.Instance;
        if (IsAchievementUnlocked() == true)
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

    protected void PublishAchievement()
    {
        if (string.IsNullOrWhiteSpace(_achievementId) == true)
        {
            return;
        }

        if (IsAchievementUnlocked() == true)
        {
            Disable();
            return;
        }

        Publisher.TryPublish(context => new AchievementEvent(context, _achievementId));
    }

    private void OnAchievementUnlocked(AchievementDefinition definition, AchievementState state)
    {
        if (definition == null || definition.Id != _achievementId)
        {
            return;
        }

        Disable();
    }

    private bool IsAchievementUnlocked()
    {
        if (string.IsNullOrWhiteSpace(_achievementId) == true)
        {
            return false;
        }

        AchievementManager manager = _achievementManager ?? AchievementManager.Instance;
        if (manager == null)
        {
            return false;
        }

        return manager.TryGetState(_achievementId, out AchievementState state) && state.IsUnlocked == true;
    }

    protected abstract void Subscribe(GameEventHub hub, CompositeSubscription subscriptions);

    protected virtual void Reset()
    {
    }
}
