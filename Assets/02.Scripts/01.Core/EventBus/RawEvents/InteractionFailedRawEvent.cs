using UnityEngine;

public readonly struct InteractionFailedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public string Message { get; }

    public InteractionFailedRawEvent(GameEventContext context, string message)
    {
        Context = context;
        Message = message;
    }
}
