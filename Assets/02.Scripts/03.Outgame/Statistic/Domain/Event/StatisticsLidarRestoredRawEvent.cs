public readonly struct StatisticsLidarRestoredRawEvent : IGameEvent
{
    public GameEventContext Context { get; }

    public StatisticsLidarRestoredRawEvent(GameEventContext context)
    {
        Context = context;
    }
}
