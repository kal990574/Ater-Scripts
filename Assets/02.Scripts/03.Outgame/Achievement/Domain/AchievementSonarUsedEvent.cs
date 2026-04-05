public readonly struct AchievementSonarUsedEvent : IAchievementEvent
{
    public GameEventContext SourceContext { get; }

    public AchievementSonarUsedEvent(GameEventContext sourceContext)
    {
        SourceContext = sourceContext;
    }
}
