using System;

public readonly struct SubJumpScareTriggeredRawEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public SubJumpScareSelectionResult  Result { get; }

    public SubJumpScareTriggeredRawEvent(
        GameEventContext context,
        SubJumpScareSelectionResult result)
    {
        Context = context;
        Result = result;
    }
}