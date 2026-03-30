public readonly struct UseInteractEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public string InstanceId { get; }
    public string GameObjectName { get; }

    public UseInteractEvent(GameEventContext context, string instanceId,  string gameObjectName)
    {
        Context = context;
        InstanceId = instanceId;
        GameObjectName = gameObjectName;
    }
}