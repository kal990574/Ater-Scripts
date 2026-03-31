public readonly struct MainJumpScareExecutedEvent : IGameEvent
{
    public GameEventContext Context { get; }

    public MainJumpScareExecutedEvent(GameEventContext context, string instanceId , int itemID)
    {
        Context = context;
    }
}