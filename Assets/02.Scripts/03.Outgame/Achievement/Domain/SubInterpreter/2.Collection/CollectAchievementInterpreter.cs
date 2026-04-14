using UnityEngine;

public class CollectAchievementInterpreter : GroupedAchievementSubInterpreterBase
{
    public CollectAchievementInterpreter(Object source)
        : base(source, new[]
        {
            AchievementKey.Collection_FirstRecord,
            AchievementKey.Collection_AllLogsComplete,
        })
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<StatisticsLogCollectedEvent>(OnLogCollected));
    }

    private void OnLogCollected(StatisticsLogCollectedEvent @event)
    {
        StatisticsManager statisticsManager = StatisticsManager.Instance;
        if (statisticsManager == null)
        {
            return;
        }

        int collectedLogCount = statisticsManager.GetCollectedLogCount();
        CheckFirstRecord(collectedLogCount);
        CheckAllLogsComplete(collectedLogCount);
    }

    private void CheckFirstRecord(int collectedLogCount)
    {
        if (ShouldCheckAchievement(AchievementKey.Collection_FirstRecord) == false)
        {
            return;
        }

        EvaluateThresholdAchievement(AchievementKey.Collection_FirstRecord, collectedLogCount);
    }

    private void CheckAllLogsComplete(int collectedLogCount)
    {
        if (ShouldCheckAchievement(AchievementKey.Collection_AllLogsComplete) == false)
        {
            return;
        }

        EvaluateThresholdAchievement(AchievementKey.Collection_AllLogsComplete, collectedLogCount);
    }
}
