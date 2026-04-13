// OnAdaptedToDarknessAchievementInterpreter.cs
using UnityEngine;

public class OnAdaptedToDarknessAchievementInterpreter : AchievementSubInterpreterBase
{
    public OnAdaptedToDarknessAchievementInterpreter(Object source)
        : base(source, AchievementKey.Mechanic_AdaptedToDarkness)
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<AchievementRunEndedRawEvent>(OnRunEnded));
    }

    private void OnRunEnded(AchievementRunEndedRawEvent rawEvent)
    {
        if (rawEvent.EndReason != EAchievementRunEndReason.Clear)
        {
            return;
        }

        if (rawEvent.Summary.SonarUseCount <= 15)
        {
            PublishAchievement();
        }
    }
}