// OnEndingReachedAchievementInterpreter.cs
using UnityEngine;

public class OnEndingReachedAchievementInterpreter : AchievementSubInterpreterBase
{
    public OnEndingReachedAchievementInterpreter(Object source)
        : base(source, AchievementKey.Story_EndingReached)
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

        PublishAchievement();
    }
}