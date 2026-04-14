using UnityEngine;

public class OnFirstLogAchievementInterpreter : AchievementSubInterpreterBase
{
    public OnFirstLogAchievementInterpreter(Object source)
        : base(source, AchievementKey.Collection_FirstRecord)
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<StatisticsLogCollectedEvent>(OnLogCollected));
    }

    private void OnLogCollected(StatisticsLogCollectedEvent @event)
    {
        AchievementManager manager = AchievementManager.Instance;
        if (manager == null)
        {
            return;
        }

        if (manager.TryGetState(AchievementKey.Collection_FirstRecord, out AchievementState state) == false)
        {
            return;
        }

        if (state.IsUnlocked == true)
        {
            return;
        }

        PublishAchievement();
    }
}
