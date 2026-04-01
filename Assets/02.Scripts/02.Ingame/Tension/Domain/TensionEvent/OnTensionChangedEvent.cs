/// <summary>
/// 텐션 증가
/// </summary>
public readonly struct OnTensionChangedEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public string Reason { get; }

    public OnTensionChangedEvent(GameEventContext context, string reason)
    {
        Context = context;
        Reason = reason;
    }
}