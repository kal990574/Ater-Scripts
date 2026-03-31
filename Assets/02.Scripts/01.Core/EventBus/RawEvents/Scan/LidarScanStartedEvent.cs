public readonly struct LidarScanStartedEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public LidarScanStartedEvent(GameEventContext context)
    {
        Context = context;
    }
}
