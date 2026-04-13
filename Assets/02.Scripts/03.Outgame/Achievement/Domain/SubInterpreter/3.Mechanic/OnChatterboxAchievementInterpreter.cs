// OnChatterboxAchievementInterpreter.cs
using UnityEngine;

public class OnChatterboxAchievementInterpreter : AchievementSubInterpreterBase
{
    public OnChatterboxAchievementInterpreter(Object source)
        : base(source, AchievementKey.Mechanic_Chatterbox)
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<AiHintAnsweredRawEvent>(OnAiHintAnswered));
    }

    private void OnAiHintAnswered(AiHintAnsweredRawEvent rawEvent)
    {
        AchievementManager manager = AchievementManager.Instance;
        if (manager == null)
        {
            return;
        }

        manager.CurrentRun.IncrementAiQuestionCount();

        if (manager.TryGetDefinition(AchievementKey.Mechanic_Chatterbox, out AchievementDefinition definition) == false)
        {
            return;
        }

        int total = manager.RunStatistics.AiQuestionCount + manager.CurrentRun.AiQuestionCount;

        if (total >= definition.TargetValue)
        {
            PublishAchievement();
        }
    }
}