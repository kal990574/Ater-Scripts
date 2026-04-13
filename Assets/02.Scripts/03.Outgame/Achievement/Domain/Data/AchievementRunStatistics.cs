using System;
using UnityEngine;

[Serializable]
public class AchievementRunStatistics
{
    [SerializeField] private int _sonarUseCount = 0;
    [SerializeField] private int _lidarRestoreCount = 0;
    [SerializeField] private int _aiQuestionCount = 0;

    public int SonarUseCount => _sonarUseCount;
    public int LidarRestoreCount => _lidarRestoreCount;
    public int AiQuestionCount => _aiQuestionCount;

    public void IncrementSonarUseCount()
    {
        _sonarUseCount++;
    }

    public void IncrementLidarRestoreCount()
    {
        _lidarRestoreCount++;
    }

    public void IncrementAiQuestionCount()
    {
        _aiQuestionCount++;
    }

    public void ResetAll()
    {
        _sonarUseCount = 0;
        _lidarRestoreCount = 0;
        _aiQuestionCount = 0;
    }
}
