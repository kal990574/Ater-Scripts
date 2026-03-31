public readonly struct SonarUsedEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public SonarUsedEvent(GameEventContext context, string instanceId)
    {
        Context = context;
    }
}
