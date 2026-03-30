public readonly struct ItemGetEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public string InstanceId { get; }
    public int ItemId { get; }

    public ItemGetEvent(GameEventContext context, string instanceId , int itemID)
    {
        Context = context;
        InstanceId = instanceId;
        ItemId = itemID;
    }
}