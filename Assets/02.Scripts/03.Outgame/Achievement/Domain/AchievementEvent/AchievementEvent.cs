// AchievementEvent.cs
public readonly struct AchievementEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public string AchievementId { get; }

    public AchievementEvent(GameEventContext context, string achievementId)
    {
        Context = context;
        AchievementId = string.IsNullOrWhiteSpace(achievementId) ? AchievementKey.None : achievementId;
    }
}