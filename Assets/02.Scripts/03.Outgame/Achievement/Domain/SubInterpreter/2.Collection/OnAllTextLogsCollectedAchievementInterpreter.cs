// OnAllTextLogsCollectedAchievementInterpreter.cs
using UnityEngine;

public class OnAllTextLogsCollectedAchievementInterpreter : AchievementSubInterpreterBase
{
    public OnAllTextLogsCollectedAchievementInterpreter(Object source)
        : base(source, AchievementKey.Collection_TextLogsComplete)
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

        if (manager.TryGetDefinition(AchievementKey.Collection_TextLogsComplete, out AchievementDefinition definition) == false)
        {
            return;
        }

        bool alreadyCollected = manager.HasCollectedLog(rawEvent.LogId);
        int predictedTextCount = manager.GetCollectedTextLogCount();

        if (rawEvent.IsTextLog == true && alreadyCollected == false)
        {
            predictedTextCount++;
        }

        if (predictedTextCount >= definition.TargetValue)
        {
            PublishAchievement();
        }
    }
}