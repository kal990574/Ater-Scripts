public readonly struct AchievementLidarTargetCompletedEvent : IAchievementEvent
{
    public GameEventContext SourceContext { get; }

    public AchievementLidarTargetCompletedEvent(GameEventContext sourceContext)
    {
        SourceContext = sourceContext;
    }
}
