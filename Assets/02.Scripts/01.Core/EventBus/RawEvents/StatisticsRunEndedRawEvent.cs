public readonly struct StatisticsRunEndedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public StatisticsRunSummary Summary { get; }
    public EStatisticsRunEndReason EndReason { get; }

    public StatisticsRunEndedRawEvent(
        GameEventContext context,
        StatisticsRunSummary summary,
        EStatisticsRunEndReason endReason)
    {
        Context = context;
        Summary = summary;
        EndReason = endReason;
    }
}
