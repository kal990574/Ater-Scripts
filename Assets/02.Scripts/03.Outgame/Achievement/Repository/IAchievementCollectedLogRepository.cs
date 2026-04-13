// IAchievementCollectedLogRepository.cs
public interface IAchievementCollectedLogRepository
{
    AchievementCollectedLogState Load();
    void Save(AchievementCollectedLogState state);
    void Reset();
}