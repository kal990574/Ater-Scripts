using UnityEngine;

public class OnFirstSignalAchievementInterpreter : AchievementSubInterpreterBase
{
    public OnFirstSignalAchievementInterpreter(Object source)
        : base(source, AchievementKey.Mechanic_FirstSignal)
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<StatisticsSonarUsedRawEvent>(OnSonarUsed));
    }

    private void OnSonarUsed(StatisticsSonarUsedRawEvent rawEvent)
    {
        PublishAchievement();
    }
}
