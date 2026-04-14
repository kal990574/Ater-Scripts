public readonly struct AiHintAnsweredRawEvent : IGameEvent
{
    public GameEventContext Context { get; }

    public AiHintAnsweredRawEvent(GameEventContext context)
    {
        Context = context;
    }
}