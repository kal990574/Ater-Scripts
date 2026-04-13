using UnityEngine;

public class StatisticsRepository : IStatisticsRepository
{
    private const string SaveKey = "Statistics.Persistent";

    public PersistentStatistics Load()
    {
        if (PlayerPrefs.HasKey(SaveKey) == false)
        {
            return new PersistentStatistics();
        }

        string json = PlayerPrefs.GetString(SaveKey, string.Empty);
        if (string.IsNullOrWhiteSpace(json) == true)
        {
            return new PersistentStatistics();
        }

        StatisticsSaveData saveData = JsonUtility.FromJson<StatisticsSaveData>(json);
        if (saveData == null || saveData.PersistentStatistics == null)
        {
            return new PersistentStatistics();
        }

        return saveData.PersistentStatistics;
    }

    public void Save(PersistentStatistics statistics)
    {
        StatisticsSaveData saveData = new StatisticsSaveData(statistics);
        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public void Reset()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
    }
}
