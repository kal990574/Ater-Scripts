using UnityEngine;

public class OnNoAiClearAchievementInterpreter : AchievementSubInterpreterBase
{
    public OnNoAiClearAchievementInterpreter(Object source)
        : base(source, AchievementKey.Challenge_NoAiClear)
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<StatisticsRunEndedRawEvent>(OnRunEnded));
    }

    private void OnRunEnded(StatisticsRunEndedRawEvent rawEvent)
    {
        if (rawEvent.EndReason != EStatisticsRunEndReason.Clear)
        {
            return;
        }

        if (rawEvent.Summary.AiQuestionCount == 0)
        {
            PublishAchievement();
        }
    }
}