public readonly struct StatisticsSonarUsedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }

    public StatisticsSonarUsedRawEvent(GameEventContext context)
    {
        Context = context;
    }
}
