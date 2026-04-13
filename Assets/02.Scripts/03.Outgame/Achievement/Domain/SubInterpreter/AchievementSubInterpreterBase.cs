// AchievementSubInterpreterBase.cs
using UnityEngine;

public abstract class AchievementSubInterpreterBase : IAchievementSubInterpreter
{
    protected readonly GameEventPublisher Publisher = new GameEventPublisher();

    private readonly string _achievementId;
    private CompositeSubscription _subscriptions;
    private bool _isEnabled;

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

        GameEventHub hub = GameEventHub.Instance;
        if (hub == null)
        {
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

        _subscriptions.Dispose();
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

        Publisher.TryPublish(context => new AchievementEvent(context, _achievementId));
    }

    protected abstract void Subscribe(GameEventHub hub, CompositeSubscription subscriptions);

    protected virtual void Reset()
    {
    }
}