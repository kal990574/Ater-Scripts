public readonly struct AchievementAiQuestionUsedEvent : IAchievementEvent
{
    public GameEventContext SourceContext { get; }

    public AchievementAiQuestionUsedEvent(GameEventContext sourceContext)
    {
        SourceContext = sourceContext;
    }
}
