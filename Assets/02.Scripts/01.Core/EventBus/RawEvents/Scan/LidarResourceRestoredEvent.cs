public readonly struct LidarResourceRestoredEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public LidarResourceRestoredEvent(GameEventContext context, string instanceId)
    {
        Context = context;
    }
}
