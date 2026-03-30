public readonly struct ReleaseInteractEvent
{
    public GameEventContext Context { get; }
    public string InstanceId { get; }
    public int ItemId { get; }

    public ReleaseInteractEvent(GameEventContext context, string instanceId , int itemID)
    {
        Context = context;
        InstanceId = instanceId;
        ItemId = itemID;
    }
}