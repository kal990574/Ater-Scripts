public interface IAchievementEvent
{
    GameEventContext Context { get; }
    string AchievementId { get; }
}