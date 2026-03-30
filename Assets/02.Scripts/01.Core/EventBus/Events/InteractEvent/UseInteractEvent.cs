public readonly struct UseInteractEvent
{
    public GameEventContext Context { get; }
    public string InstanceId { get; }
    public int ItemId { get; }

    public UseInteractEvent(GameEventContext context, string instanceId , int itemID)
    {
        Context = context;
        InstanceId = instanceId;
        ItemId = itemID;
    }
}