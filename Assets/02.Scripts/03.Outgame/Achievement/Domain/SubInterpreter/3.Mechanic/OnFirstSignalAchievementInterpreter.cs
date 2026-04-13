// OnFirstSignalAchievementInterpreter.cs
using UnityEngine;

public class OnFirstSignalAchievementInterpreter : AchievementSubInterpreterBase
{
    public OnFirstSignalAchievementInterpreter(Object source)
        : base(source, AchievementKey.Mechanic_FirstSignal)
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<SonarScanStartedRawEvent>(OnSonarScanStarted));
    }

    private void OnSonarScanStarted(SonarScanStartedRawEvent rawEvent)
    {
        AchievementManager manager = AchievementManager.Instance;
        if (manager != null)
        {
            manager.CurrentRun.IncrementSonarUseCount();
        }

        PublishAchievement();
    }
}