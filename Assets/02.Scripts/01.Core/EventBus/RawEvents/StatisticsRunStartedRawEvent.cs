public struct StatisticsRunStartedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }

    public StatisticsRunStartedRawEvent(
        GameEventContext context)
    {
        Context = context;
    }
}