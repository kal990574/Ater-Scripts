public readonly struct ScannableScanStartEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public string TargetInstanceID { get; }
    public ScannableScanStartEvent(GameEventContext context, string instanceId)
    {
        Context = context;
        TargetInstanceID = instanceId;
    }
}
