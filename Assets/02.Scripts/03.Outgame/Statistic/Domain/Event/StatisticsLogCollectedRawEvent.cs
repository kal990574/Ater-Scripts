public readonly struct StatisticsLogCollectedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public int ItemId { get; }

    public StatisticsLogCollectedRawEvent(GameEventContext context, int itemId)
    {
        Context = context;
        ItemId = itemId;
    }
}
