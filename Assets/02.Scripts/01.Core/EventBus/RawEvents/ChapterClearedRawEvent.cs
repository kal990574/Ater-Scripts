// ChapterClearedRawEvent.cs
public readonly struct ChapterClearedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public int ChapterId { get; }

    public ChapterClearedRawEvent(GameEventContext context, int chapterId)
    {
        Context = context;
        ChapterId = chapterId;
    }
}