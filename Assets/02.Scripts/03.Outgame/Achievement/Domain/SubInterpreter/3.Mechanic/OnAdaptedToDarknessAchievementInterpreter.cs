using UnityEngine;

public class OnAdaptedToDarknessAchievementInterpreter : AchievementSubInterpreterBase
{
    public OnAdaptedToDarknessAchievementInterpreter(Object source)
        : base(source, AchievementKey.Mechanic_AdaptedToDarkness)
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<InGameRunEndedRawEvent>(OnRunEnded));
    }

    private void OnRunEnded(InGameRunEndedRawEvent rawEvent)
    {
        if (rawEvent.EndReason != EStatisticsRunEndReason.Clear)
        {
            return;
        }

        if (rawEvent.Summary.SonarUseCount <= 15)
        {
            PublishAchievement();
        }
    }
}