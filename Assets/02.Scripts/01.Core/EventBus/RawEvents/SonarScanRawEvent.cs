public readonly struct SonarScanStartedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }

    public SonarScanStartedRawEvent(GameEventContext context)
    {
        Context = context;
    }
}