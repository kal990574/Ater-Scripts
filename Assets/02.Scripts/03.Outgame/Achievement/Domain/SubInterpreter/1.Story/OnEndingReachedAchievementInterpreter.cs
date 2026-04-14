using UnityEngine;

public class OnEndingReachedAchievementInterpreter : AchievementSubInterpreterBase
{
    public OnEndingReachedAchievementInterpreter(Object source)
        : base(source, AchievementKey.Story_EndingReached)
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

        PublishAchievement();
    }
}