public readonly struct UseInteractEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public string InstanceId { get; }
    public UseInteractEvent(GameEventContext context, string instanceId)
    {
        Context = context;
        InstanceId = instanceId;
    }
}
