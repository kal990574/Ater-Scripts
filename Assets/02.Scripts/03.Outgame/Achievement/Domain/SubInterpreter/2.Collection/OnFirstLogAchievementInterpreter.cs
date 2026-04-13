// OnFirstLogAchievementInterpreter.cs
using UnityEngine;

public class OnFirstLogAchievementInterpreter : AchievementSubInterpreterBase
{
    public OnFirstLogAchievementInterpreter(Object source)
        : base(source, AchievementKey.Collection_FirstRecord)
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<LogCollectedRawEvent>(OnLogCollected));
    }

    private void OnLogCollected(LogCollectedRawEvent rawEvent)
    {
        AchievementManager manager = AchievementManager.Instance;
        if (manager == null)
        {
            return;
        }

        bool alreadyCollected = manager.HasCollectedLog(rawEvent.LogId);
        int predictedCount = manager.GetCollectedLogCount() + (alreadyCollected == true ? 0 : 1);

        if (predictedCount == 1)
        {
            PublishAchievement();
        }
    }
}