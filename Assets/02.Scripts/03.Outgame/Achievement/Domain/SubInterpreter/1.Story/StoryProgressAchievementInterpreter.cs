using UnityEngine;

public class StoryProgressAchievementInterpreter : GroupedAchievementSubInterpreterBase
{
    public StoryProgressAchievementInterpreter(Object source)
        : base(source, new[]
        {
            AchievementKey.Story_PrologueClear,
            AchievementKey.Story_Act1Clear,
            AchievementKey.Story_Act2Clear,
            AchievementKey.Story_EndingReached
        })
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<ChapterClearedRawEvent>(OnChapterCleared));
        subscriptions.Add(hub.Subscribe<InGameRunEndedRawEvent>(OnRunEnded));
    }

    private void OnChapterCleared(ChapterClearedRawEvent rawEvent)
    {
        switch (rawEvent.ChapterId)
        {
            case 0:
                CheckPrologueClear();
                break;

            case 1:
                CheckAct1Clear();
                break;

            case 2:
                CheckAct2Clear();
                break;
        }
    }

    private void OnRunEnded(InGameRunEndedRawEvent rawEvent)
    {
        if (rawEvent.EndReason != EStatisticsRunEndReason.Clear)
        {
            return;
        }

        CheckEndingReached();
    }

    private void CheckPrologueClear()
    {
        if (ShouldCheckAchievement(AchievementKey.Story_PrologueClear) == false)
        {
            return;
        }

        PublishAchievement(AchievementKey.Story_PrologueClear);
    }

    private void CheckAct1Clear()
    {
        if (ShouldCheckAchievement(AchievementKey.Story_Act1Clear) == false)
        {
            return;
        }

        PublishAchievement(AchievementKey.Story_Act1Clear);
    }

    private void CheckAct2Clear()
    {
        if (ShouldCheckAchievement(AchievementKey.Story_Act2Clear) == false)
        {
            return;
        }

        PublishAchievement(AchievementKey.Story_Act2Clear);
    }
    

    private void CheckEndingReached()
    {
        if (ShouldCheckAchievement(AchievementKey.Story_EndingReached) == false)
        {
            return;
        }

        PublishAchievement(AchievementKey.Story_EndingReached);
    }
}
