using UnityEngine;

public class MechanicAchievementInterpreter : GroupedAchievementSubInterpreterBase
{
    public MechanicAchievementInterpreter(Object source)
        : base(source, new[]
        {
            AchievementKey.Mechanic_FirstSignal,
            AchievementKey.Mechanic_RestorationExpert,
            AchievementKey.Mechanic_Chatterbox
        })
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<StatisticsSonarUsedEvent>(OnSonarUsed));
        subscriptions.Add(hub.Subscribe<StatisticsLidarRestoredEvent>(OnLidarRestored));
        subscriptions.Add(hub.Subscribe<StatisticsAiQuestionEvent>(OnAiQuestionUsed));
    }

    private void OnSonarUsed(StatisticsSonarUsedEvent @event)
    {
        StatisticsManager statisticsManager = StatisticsManager.Instance;
        if (statisticsManager == null)
        {
            return;
        }

        CheckFirstSignal(statisticsManager.GetTotalSonarUseCount());
    }

    private void OnLidarRestored(StatisticsLidarRestoredEvent @event)
    {
        StatisticsManager statisticsManager = StatisticsManager.Instance;
        if (statisticsManager == null)
        {
            return;
        }

        CheckRestorationExpert(statisticsManager.GetTotalLidarRestoreCount());
    }

    private void OnAiQuestionUsed(StatisticsAiQuestionEvent @event)
    {
        StatisticsManager statisticsManager = StatisticsManager.Instance;
        if (statisticsManager == null)
        {
            return;
        }

        CheckChatterbox(statisticsManager.GetTotalAiQuestionCount());
    }

    private void CheckFirstSignal(int totalSonarUseCount)
    {
        if (ShouldCheckAchievement(AchievementKey.Mechanic_FirstSignal) == false)
        {
            return;
        }

        EvaluateThresholdAchievement(AchievementKey.Mechanic_FirstSignal, totalSonarUseCount);
    }

    private void CheckRestorationExpert(int totalLidarRestoreCount)
    {
        if (ShouldCheckAchievement(AchievementKey.Mechanic_RestorationExpert) == false)
        {
            return;
        }

        EvaluateThresholdAchievement(AchievementKey.Mechanic_RestorationExpert, totalLidarRestoreCount);
    }

    private void CheckChatterbox(int totalAiQuestionCount)
    {
        if (ShouldCheckAchievement(AchievementKey.Mechanic_Chatterbox) == false)
        {
            return;
        }

        EvaluateThresholdAchievement(AchievementKey.Mechanic_Chatterbox, totalAiQuestionCount);
    }
}
