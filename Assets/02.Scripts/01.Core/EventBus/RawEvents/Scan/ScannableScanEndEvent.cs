public readonly struct ScannableScanEndEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public string TargetInstanceID { get; }
    public bool IsScanComplete { get; }
    public ScannableScanEndEvent(GameEventContext context, string instanceId, bool isScanComplete)
    {
        Context = context;
        TargetInstanceID = instanceId;
        IsScanComplete = isScanComplete;
    }
}
