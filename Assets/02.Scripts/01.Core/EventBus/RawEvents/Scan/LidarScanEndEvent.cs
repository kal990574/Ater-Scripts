public readonly struct LidarScanEndEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public LidarScanEndEvent(GameEventContext context)
    {
        Context = context;
    }
}
