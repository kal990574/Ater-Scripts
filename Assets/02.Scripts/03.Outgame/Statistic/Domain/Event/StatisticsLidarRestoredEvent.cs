public readonly struct StatisticsLidarRestoredEvent : IGameEvent
{
    public GameEventContext Context { get; }

    public StatisticsLidarRestoredEvent(GameEventContext context)
    {
        Context = context;
    }
}
