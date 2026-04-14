using UnityEngine;

public class ChallengeAchievementInterpreter : GroupedAchievementSubInterpreterBase
{
    private bool _isRunning;

    public ChallengeAchievementInterpreter(Object source)
        : base(source, new[]
        {
            AchievementKey.Challenge_AdaptedToDarkness,
            AchievementKey.Challenge_NoAiClear
        })
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<InGameRunStartedRawEvent>(OnRunStarted));
        subscriptions.Add(hub.Subscribe<InGameRunEndedRawEvent>(OnRunEnded));
    }

    private void OnRunStarted(InGameRunStartedRawEvent rawEvent)
    {
        if (rawEvent.StartChapter != 0)
        {
            return;
        }
        _isRunning = true;
    }

    private void OnRunEnded(InGameRunEndedRawEvent rawEvent)
    {
        if (!_isRunning)
        {
            return;
        }
        if (rawEvent.EndReason != EStatisticsRunEndReason.Clear)
        {
            _isRunning = false;
            return;
        }

        CheckAdaptedToDarkness(rawEvent.Summary.SonarUseCount);
        CheckNoAiClear(rawEvent.Summary.AiQuestionCount);

        _isRunning = false;
    }

    private void CheckAdaptedToDarkness(int sonarUseCount)
    {
        if (ShouldCheckAchievement(AchievementKey.Challenge_AdaptedToDarkness) == false)
        {
            return;
        }

        if (sonarUseCount > 15)
        {
            return;
        }

        PublishAchievement(AchievementKey.Challenge_AdaptedToDarkness);
    }

    private void CheckNoAiClear(int aiQuestionCount)
    {
        if (ShouldCheckAchievement(AchievementKey.Challenge_NoAiClear) == false)
        {
            return;
        }

        if (_isRunning == false || aiQuestionCount != 0)
        {
            return;
        }

        PublishAchievement(AchievementKey.Challenge_NoAiClear);
    }

    protected override void Reset()
    {
        _isRunning = false;
    }
}
