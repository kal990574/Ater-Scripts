// AchievementRunEndedRawEvent.cs
public readonly struct AchievementRunEndedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public AchievementRunSummary Summary { get; }
    public EAchievementRunEndReason EndReason { get; }

    public AchievementRunEndedRawEvent(
        GameEventContext context,
        AchievementRunSummary summary,
        EAchievementRunEndReason endReason)
    {
        Context = context;
        Summary = summary;
        EndReason = endReason;
    }
}