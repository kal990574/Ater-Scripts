// OnNoAiClearAchievementInterpreter.cs
using UnityEngine;

public class OnNoAiClearAchievementInterpreter : AchievementSubInterpreterBase
{
    public OnNoAiClearAchievementInterpreter(Object source)
        : base(source, AchievementKey.Challenge_NoAiClear)
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

        if (rawEvent.Summary.AiQuestionCount == 0)
        {
            PublishAchievement();
        }
    }
}