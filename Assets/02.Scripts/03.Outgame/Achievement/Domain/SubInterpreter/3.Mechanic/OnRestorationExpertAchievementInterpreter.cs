using UnityEngine;

public class OnRestorationExpertAchievementInterpreter : AchievementSubInterpreterBase
{
    public OnRestorationExpertAchievementInterpreter(Object source)
        : base(source, AchievementKey.Mechanic_RestorationExpert)
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<StatisticsLidarRestoredEvent>(OnLidarRestored));
    }

    private void OnLidarRestored(StatisticsLidarRestoredEvent @event)
    {
        StatisticsManager statisticsManager = StatisticsManager.Instance;
        AchievementManager achievementManager = AchievementManager.Instance;

        if (statisticsManager == null || achievementManager == null)
        {
            return;
        }

        if (achievementManager.TryGetDefinition(AchievementKey.Mechanic_RestorationExpert, out AchievementDefinition definition) == false)
        {
            return;
        }

        int total = statisticsManager.GetTotalLidarRestoreCount();

        if (total >= definition.TargetValue)
        {
            PublishAchievement();
        }
    }
}
