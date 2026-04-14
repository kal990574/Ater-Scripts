public struct InGameRunStartedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }

    public InGameRunStartedRawEvent(
        GameEventContext context)
    {
        Context = context;
    }
}