using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PersistentStatistics
{
    [SerializeField] private int _sonarUseCount;
    [SerializeField] private int _lidarRestoreCount;
    [SerializeField] private int _aiQuestionCount;
    [SerializeField] private List<int> _collectedLogItemIds = new List<int>();

    public int SonarUseCount => _sonarUseCount;
    public int LidarRestoreCount => _lidarRestoreCount;
    public int AiQuestionCount => _aiQuestionCount;
    public IReadOnlyList<int> CollectedLogItemIds => _collectedLogItemIds;

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

    public bool TryAddCollectedLogItemId(int itemId)
    {
        if (itemId <= 0)
        {
            return false;
        }

        if (_collectedLogItemIds.Contains(itemId) == true)
        {
            return false;
        }

        _collectedLogItemIds.Add(itemId);
        return true;
    }

    public int GetCollectedLogCount()
    {
        return _collectedLogItemIds.Count;
    }

    public bool RemoveCollectedLogItemId(int itemId)
    {
        return _collectedLogItemIds.Remove(itemId);
    }

    public void ClearCollectedLogItemIds()
    {
        _collectedLogItemIds.Clear();
    }

    public void SetCounts(int sonarUseCount, int lidarRestoreCount, int aiQuestionCount)
    {
        _sonarUseCount = Mathf.Max(0, sonarUseCount);
        _lidarRestoreCount = Mathf.Max(0, lidarRestoreCount);
        _aiQuestionCount = Mathf.Max(0, aiQuestionCount);
    }

    public void ResetAll()
    {
        _sonarUseCount = 0;
        _lidarRestoreCount = 0;
        _aiQuestionCount = 0;
        _collectedLogItemIds.Clear();
    }
}
