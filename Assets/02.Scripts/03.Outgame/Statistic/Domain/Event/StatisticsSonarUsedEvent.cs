public readonly struct StatisticsSonarUsedEvent : IGameEvent
{
    public GameEventContext Context { get; }

    public StatisticsSonarUsedEvent(GameEventContext context)
    {
        Context = context;
    }
}
