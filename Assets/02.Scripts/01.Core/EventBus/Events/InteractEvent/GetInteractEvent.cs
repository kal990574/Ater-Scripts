public readonly struct GetInteractEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public string InstanceId { get; }
    public int ItemId { get; }

    public GetInteractEvent(GameEventContext context, string instanceId , int itemID)
    {
        Context = context;
        InstanceId = instanceId;
        ItemId = itemID;
    }
}