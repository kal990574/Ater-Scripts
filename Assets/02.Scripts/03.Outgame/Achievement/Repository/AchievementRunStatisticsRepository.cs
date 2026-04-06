using UnityEngine;

public class AchievementRunStatisticsRepository : IAchievementRunStatisticsRepository
{
    private const string SaveKey = "Achievement.RunStatistics";

    public AchievementRunStatistics Load()
    {
        if (PlayerPrefs.HasKey(SaveKey) == false)
        {
            return new AchievementRunStatistics();
        }

        string json = PlayerPrefs.GetString(SaveKey, string.Empty);

        if (string.IsNullOrEmpty(json) == true)
        {
            return new AchievementRunStatistics();
        }

        AchievementRunStatistics statistics = JsonUtility.FromJson<AchievementRunStatistics>(json);

        if (statistics == null)
        {
            return new AchievementRunStatistics();
        }

        return statistics;
    }

    public void Save(AchievementRunStatistics statistics)
    {
        if (statistics == null)
        {
            return;
        }

        string json = JsonUtility.ToJson(statistics);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public void Reset()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
    }
}
