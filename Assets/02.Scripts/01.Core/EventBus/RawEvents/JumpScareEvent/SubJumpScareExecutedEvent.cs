public readonly struct SubJumpScareExecutedEvent : IGameEvent
{
    public GameEventContext Context { get; }

    public SubJumpScareExecutedEvent(GameEventContext context, string instanceId , int itemID)
    {
        Context = context;
    }
}