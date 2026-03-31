public readonly struct ScanCompleteEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public string InstanceId { get; }
    public ScanCompleteEvent(GameEventContext context, string instanceId)
    {
        Context = context;
        InstanceId = instanceId;
    }
}
