using UnityEngine;

public class OnChatterboxAchievementInterpreter : AchievementSubInterpreterBase
{
    public OnChatterboxAchievementInterpreter(Object source)
        : base(source, AchievementKey.Mechanic_Chatterbox)
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<AiHintAnsweredRawEvent>(OnAiHintAnswered));
    }

    private void OnAiHintAnswered(AiHintAnsweredRawEvent rawEvent)
    {
        StatisticsManager statisticsManager = StatisticsManager.Instance;
        AchievementManager achievementManager = AchievementManager.Instance;

        if (statisticsManager == null || achievementManager == null)
        {
            return;
        }

        if (achievementManager.TryGetDefinition(AchievementKey.Mechanic_Chatterbox, out AchievementDefinition definition) == false)
        {
            return;
        }

        int total =
            statisticsManager.Persistent.AiQuestionCount +
            statisticsManager.CurrentRun.AiQuestionCount + 1;

        if (total >= definition.TargetValue)
        {
            PublishAchievement();
        }
    }
}