/// <summary>
/// 텐션 증가
/// </summary>
public readonly struct OnTensionChangedEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public string Reason { get; }
    public ETensionChannel Channel { get; }
    public float Amount { get; }

    public OnTensionChangedEvent(GameEventContext context, string reason ,ETensionChannel channel, float amount)
    {
        Context = context;
        Reason = reason;
        Channel = channel;
        Amount = amount;
    }
}