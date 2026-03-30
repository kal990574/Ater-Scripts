public readonly struct ScanInteractEvent
{
    public GameEventContext Context { get; }
    public string InstanceId { get; }
    public string GameObjectName { get; }

    public ScanInteractEvent(GameEventContext context, string instanceId, string gameObjectName)
    {
        Context = context;
        InstanceId = instanceId;
        GameObjectName = gameObjectName;
    }
}