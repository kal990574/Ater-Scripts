// AchievementCollectedLogRepository.cs
using UnityEngine;

public class AchievementCollectedLogRepository : IAchievementCollectedLogRepository
{
    private const string SaveKey = "Achievement.CollectedLogs";

    public AchievementCollectedLogState Load()
    {
        if (PlayerPrefs.HasKey(SaveKey) == false)
        {
            return new AchievementCollectedLogState();
        }

        string json = PlayerPrefs.GetString(SaveKey, string.Empty);

        if (string.IsNullOrEmpty(json) == true)
        {
            return new AchievementCollectedLogState();
        }

        AchievementCollectedLogState state = JsonUtility.FromJson<AchievementCollectedLogState>(json);

        if (state == null)
        {
            return new AchievementCollectedLogState();
        }

        return state;
    }

    public void Save(AchievementCollectedLogState state)
    {
        if (state == null)
        {
            return;
        }

        string json = JsonUtility.ToJson(state);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public void Reset()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
    }
}