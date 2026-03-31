public readonly struct ScanCompleteEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public string InstanceId { get; }
    public string GameObjectName { get; }

    public ScanCompleteEvent(GameEventContext context, string instanceId, string gameObjectName)
    {
        Context = context;
        InstanceId = instanceId;
        GameObjectName = gameObjectName;
    }
}