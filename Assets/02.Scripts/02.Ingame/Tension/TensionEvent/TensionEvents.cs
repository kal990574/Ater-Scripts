/// <summary>
/// 텐션 증가
/// </summary>
public readonly struct OnTensionIncreaseEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public string Reason { get; }

    public OnTensionIncreaseEvent(GameEventContext context, string reason)
    {
        Context = context;
        Reason = reason;
    }
}

//텐션 감소
public readonly struct OnTensionDecreaseEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public string Reason { get; }

    public OnTensionDecreaseEvent(GameEventContext context, string reason)
    {
        Context = context;
        Reason = reason;
    }
}