// OnAct3ClearedAchievementInterpreter.cs
using UnityEngine;

public class OnAct3ClearedAchievementInterpreter : AchievementSubInterpreterBase
{
    public OnAct3ClearedAchievementInterpreter(Object source)
        : base(source, AchievementKey.Story_Act3Clear)
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<ChapterClearedRawEvent>(OnChapterCleared));
    }

    private void OnChapterCleared(ChapterClearedRawEvent rawEvent)
    {
        if (rawEvent.ChapterId != 3)
        {
            return;
        }

        PublishAchievement();
    }
}