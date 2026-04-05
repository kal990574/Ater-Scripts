public readonly struct AchievementEndingReachedEvent : IAchievementEvent
{
    public GameEventContext SourceContext { get; }

    public AchievementEndingReachedEvent(GameEventContext sourceContext)
    {
        SourceContext = sourceContext;
    }
}
