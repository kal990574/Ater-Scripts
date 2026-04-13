// OnRestorationExpertAchievementInterpreter.cs
using UnityEngine;

public class OnRestorationExpertAchievementInterpreter : AchievementSubInterpreterBase
{
    public OnRestorationExpertAchievementInterpreter(Object source)
        : base(source, AchievementKey.Mechanic_RestorationExpert)
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<LidarScanTargetCompletedRawEvent>(OnLidarCompleted));
    }

    private void OnLidarCompleted(LidarScanTargetCompletedRawEvent rawEvent)
    {
        AchievementManager manager = AchievementManager.Instance;
        if (manager == null)
        {
            return;
        }

        manager.CurrentRun.IncrementLidarRestoreCount();

        if (manager.TryGetDefinition(AchievementKey.Mechanic_RestorationExpert, out AchievementDefinition definition) == false)
        {
            return;
        }

        int total = manager.RunStatistics.LidarRestoreCount + manager.CurrentRun.LidarRestoreCount;

        if (total >= definition.TargetValue)
        {
            PublishAchievement();
        }
    }
}