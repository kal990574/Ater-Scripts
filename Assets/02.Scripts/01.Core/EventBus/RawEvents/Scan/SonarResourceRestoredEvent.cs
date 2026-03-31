public readonly struct SonarResourceRestoredEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public SonarResourceRestoredEvent(GameEventContext context, string instanceId)
    {
        Context = context;
    }
}
