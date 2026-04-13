using UnityEngine;

public class OnAct1ClearedAchievementInterpreter : AchievementSubInterpreterBase
{
    public OnAct1ClearedAchievementInterpreter(Object source)
        : base(source, AchievementKey.Story_Act1Clear)
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<ChapterClearedRawEvent>(OnChapterCleared));
    }

    private void OnChapterCleared(ChapterClearedRawEvent rawEvent)
    {
        if (rawEvent.ChapterId != 1)
        {
            return;
        }

        PublishAchievement();
    }
}