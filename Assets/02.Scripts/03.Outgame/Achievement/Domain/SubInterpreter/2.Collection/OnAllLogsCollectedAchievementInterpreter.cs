using UnityEngine;

public class OnAllLogsCollectedAchievementInterpreter : AchievementSubInterpreterBase
{
    public OnAllLogsCollectedAchievementInterpreter(Object source)
        : base(source, AchievementKey.Collection_AllLogsComplete)
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<ItemAcquiredRawEvent>(OnItemAcquired));
    }

    private void OnItemAcquired(ItemAcquiredRawEvent rawEvent)
    {
        if (rawEvent.ItemType != EItemType.Log)
        {
            return;
        }

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

        if (statisticsManager.HasCollectedLog(rawEvent.ItemId) == false)
        {
            predictedCount++;
        }

        if (predictedCount >= definition.TargetValue)
        {
            PublishAchievement();
        }
    }
}