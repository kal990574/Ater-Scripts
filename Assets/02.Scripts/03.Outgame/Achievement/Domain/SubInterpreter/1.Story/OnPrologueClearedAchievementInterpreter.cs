using UnityEngine;

//튜토리얼 클리어
public class OnPrologueClearedAchievementInterpreter : AchievementSubInterpreterBase
{
    public OnPrologueClearedAchievementInterpreter(Object source)
        : base(source, AchievementKey.Story_PrologueClear)
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<ChapterClearedRawEvent>(OnChapterCleared));
    }

    private void OnChapterCleared(ChapterClearedRawEvent rawEvent)
    {
        if (rawEvent.ChapterId != 0)
        {
            return;
        }

        PublishAchievement();
    }
}