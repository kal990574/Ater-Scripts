public readonly struct StatisticsAiQuestionEvent : IGameEvent
{
    public GameEventContext Context { get; }

    public StatisticsAiQuestionEvent(GameEventContext context)
    {
        Context = context;
    }
}
