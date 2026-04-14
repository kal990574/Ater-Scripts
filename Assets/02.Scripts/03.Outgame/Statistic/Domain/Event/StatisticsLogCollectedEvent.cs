public readonly struct StatisticsLogCollectedEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public int ItemId { get; }

    public StatisticsLogCollectedEvent(GameEventContext context, int itemId)
    {
        Context = context;
        ItemId = itemId;
    }
}
