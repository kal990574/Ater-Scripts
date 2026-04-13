using System;
using UnityEngine;

//현재까지의 영구 히스토리 데이터
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

    public void AddSonarUseCount(int value)
    {
        if (value <= 0)
        {
            return;
        }

        _sonarUseCount += value;
    }

    public void AddLidarRestoreCount(int value)
    {
        if (value <= 0)
        {
            return;
        }

        _lidarRestoreCount += value;
    }

    public void AddAiQuestionCount(int value)
    {
        if (value <= 0)
        {
            return;
        }

        _aiQuestionCount += value;
    }

    public void ResetAll()
    {
        _sonarUseCount = 0;
        _lidarRestoreCount = 0;
        _aiQuestionCount = 0;
    }
}