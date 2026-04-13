using System;
using UnityEngine;

[Serializable]
public class CurrentRunStatistics
{
    [SerializeField] private int _sonarUseCount;
    [SerializeField] private int _lidarRestoreCount;
    [SerializeField] private int _aiQuestionCount;
    [SerializeField] private float _startTime;
    [SerializeField] private float _endTime;

    public int SonarUseCount => _sonarUseCount;
    public int LidarRestoreCount => _lidarRestoreCount;
    public int AiQuestionCount => _aiQuestionCount;
    public float StartTime => _startTime;
    public float EndTime => _endTime;

    public void Begin()
    {
        _sonarUseCount = 0;
        _lidarRestoreCount = 0;
        _aiQuestionCount = 0;
        _startTime = Time.time;
        _endTime = 0f;
    }

    public void MarkEnded()
    {
        _endTime = Time.time;
    }

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

    public void SetCounts(int sonarUseCount, int lidarRestoreCount, int aiQuestionCount)
    {
        _sonarUseCount = Mathf.Max(0, sonarUseCount);
        _lidarRestoreCount = Mathf.Max(0, lidarRestoreCount);
        _aiQuestionCount = Mathf.Max(0, aiQuestionCount);
    }
}
