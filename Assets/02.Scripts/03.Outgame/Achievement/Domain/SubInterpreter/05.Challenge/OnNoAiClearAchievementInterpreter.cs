using UnityEngine;

public class OnNoAiClearAchievementInterpreter : AchievementSubInterpreterBase
{
    private bool _isRunning = false;
    public OnNoAiClearAchievementInterpreter(Object source)
        : base(source, AchievementKey.Challenge_NoAiClear)
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<StatisticsRunStartedRawEvent>(OnRunStarted));
        subscriptions.Add(hub.Subscribe<StatisticsRunEndedRawEvent>(OnRunEnded));
    }

    private void OnRunStarted(StatisticsRunStartedRawEvent rawEvent)
    {
        _isRunning = true;
    }
    
    private void OnRunEnded(StatisticsRunEndedRawEvent rawEvent)
    {
        if (rawEvent.EndReason != EStatisticsRunEndReason.Clear)
        {
            return;
        }

        if (_isRunning && rawEvent.Summary.AiQuestionCount == 0)
        {
            PublishAchievement();
        }

        _isRunning = false;
    }
}