// OnAct2ClearedAchievementInterpreter.cs
using UnityEngine;

public class OnAct2ClearedAchievementInterpreter : AchievementSubInterpreterBase
{
    public OnAct2ClearedAchievementInterpreter(Object source)
        : base(source, AchievementKey.Story_Act2Clear)
    {
    }

    protected override void Subscribe(GameEventHub hub, CompositeSubscription subscriptions)
    {
        subscriptions.Add(hub.Subscribe<ChapterClearedRawEvent>(OnChapterCleared));
    }

    private void OnChapterCleared(ChapterClearedRawEvent rawEvent)
    {
        if (rawEvent.ChapterId != 2)
        {
            return;
        }

        PublishAchievement();
    }
}