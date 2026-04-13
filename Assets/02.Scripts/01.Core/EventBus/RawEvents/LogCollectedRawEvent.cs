// LogCollectedRawEvent.cs
public readonly struct LogCollectedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public string LogId { get; }
    public bool IsTextLog { get; }

    public LogCollectedRawEvent(GameEventContext context, string logId, bool isTextLog)
    {
        Context = context;
        LogId = logId ?? string.Empty;
        IsTextLog = isTextLog;
    }
}