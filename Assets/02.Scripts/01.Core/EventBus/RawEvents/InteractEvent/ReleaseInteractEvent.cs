public readonly struct ReleaseInteractEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public string InstanceId { get; }
    public int ItemId { get; }
    public string GameObjectName { get; }
    public ReleaseInteractEvent(GameEventContext context, string instanceId , int itemID, string gameObjectName)
    {
        Context = context;
        InstanceId = instanceId;
        ItemId = itemID;
        GameObjectName = gameObjectName;
    }
}