public struct InGameRunStartedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public int StartChapter { get; }
    public InGameRunStartedRawEvent(
        GameEventContext context, int startChapter)
    {
        Context = context;
        StartChapter = startChapter;
    }
}