public readonly struct LidarResourceDepletedEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public LidarResourceDepletedEvent(GameEventContext context, string instanceId)
    {
        Context = context;
    }
}
