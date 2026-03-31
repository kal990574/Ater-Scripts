public readonly struct StressSpikeEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public float Amount { get; }
    public string Reason { get; }

    public StressSpikeEvent(GameEventContext context, float amount, string reason)
    {
        Context = context;
        Amount = amount;
        Reason = reason;
    }
}