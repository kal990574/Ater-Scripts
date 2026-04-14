public readonly struct InGameRunEndedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public StatisticsRunSummary Summary { get; }
    public EStatisticsRunEndReason EndReason { get; }

    public InGameRunEndedRawEvent(
        GameEventContext context,
        StatisticsRunSummary summary,
        EStatisticsRunEndReason endReason)
    {
        Context = context;
        Summary = summary;
        EndReason = endReason;
    }
}
