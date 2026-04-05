public interface IAchievementRunStatisticsRepository
{
    AchievementRunStatistics Load();
    void Save(AchievementRunStatistics statistics);
    void Reset();
}
