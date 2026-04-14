public interface IStatisticsRepository
{
    PersistentStatistics Load();
    void Save(PersistentStatistics statistics);
    void Reset();
}
