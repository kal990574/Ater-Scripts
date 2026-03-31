public readonly struct SonarResourceDepletedEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public SonarResourceDepletedEvent(GameEventContext context, string instanceId)
    {
        Context = context;
    }
}
