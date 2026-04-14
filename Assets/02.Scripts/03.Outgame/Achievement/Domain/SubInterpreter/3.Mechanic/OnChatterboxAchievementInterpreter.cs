using UnityEngine;

public class OnChatterboxAchievementInterpreter : AchievementSubInterpreterBase
{
    public OnChatterboxAchievementInterpreter(Object source)
        : base(source, AchievementKey.Mechanic_Chatterbox)
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<StatisticsAiQuestionEvent>(OnAiQuestionUsed));
    }

    private void OnAiQuestionUsed(StatisticsAiQuestionEvent @event)
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

        int total = statisticsManager.GetTotalAiQuestionCount();

        if (total >= definition.TargetValue)
        {
            PublishAchievement();
        }
    }
}
