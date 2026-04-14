public readonly struct StatisticsAiQuestionRawEvent : IGameEvent
{
    public GameEventContext Context { get; }

    public StatisticsAiQuestionRawEvent(GameEventContext context)
    {
        Context = context;
    }
}
