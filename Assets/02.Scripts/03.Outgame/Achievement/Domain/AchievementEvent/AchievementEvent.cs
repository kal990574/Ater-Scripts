public readonly struct AchievementEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public string Key { get; }

    public AchievementEvent(GameEventContext sourceContext,  string key)
    {
        Context = sourceContext;
        Key = key;
    }
}
