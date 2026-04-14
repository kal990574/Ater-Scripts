using UnityEngine;

public class OnAllLogsCollectedAchievementInterpreter : AchievementSubInterpreterBase
{
    public OnAllLogsCollectedAchievementInterpreter(Object source)
        : base(source, AchievementKey.Collection_AllLogsComplete)
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<StatisticsLogCollectedEvent>(OnLogCollected));
    }

    private void OnLogCollected(StatisticsLogCollectedEvent @event)
    {
        StatisticsManager statisticsManager = StatisticsManager.Instance;
        AchievementManager achievementManager = AchievementManager.Instance;

        if (statisticsManager == null || achievementManager == null)
        {
            return;
        }

        if (achievementManager.TryGetDefinition(AchievementKey.Collection_AllLogsComplete, out AchievementDefinition definition) == false)
        {
            return;
        }

        int predictedCount = statisticsManager.GetCollectedLogCount();

        if (predictedCount >= definition.TargetValue)
        {
            PublishAchievement();
        }
    }
}
