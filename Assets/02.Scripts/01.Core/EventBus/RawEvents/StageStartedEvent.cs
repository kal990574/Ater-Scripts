public readonly struct StageStartedEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public StageStartedEvent(GameEventContext context, string instanceId)
    {
        Context = context;
    }
}
