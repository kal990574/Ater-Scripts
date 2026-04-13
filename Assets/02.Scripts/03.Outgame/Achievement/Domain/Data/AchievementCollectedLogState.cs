using System;
using System.Collections.Generic;
using UnityEngine;

//현재까지 모은 로그들의 아이디
[Serializable]
public class AchievementCollectedLogState
{
    [SerializeField] private List<string> _allCollectedLogIds = new List<string>();
    [SerializeField] private List<string> _textCollectedLogIds = new List<string>();

    public int AllCollectedCount => _allCollectedLogIds.Count;
    public int TextCollectedCount => _textCollectedLogIds.Count;

    public bool HasCollected(string logId)
    {
        if (string.IsNullOrWhiteSpace(logId) == true)
        {
            return false;
        }

        return _allCollectedLogIds.Contains(logId);
    }

    public bool TryCollect(string logId, bool isTextLog)
    {
        if (string.IsNullOrWhiteSpace(logId) == true)
        {
            return false;
        }

        if (_allCollectedLogIds.Contains(logId) == true)
        {
            return false;
        }

        _allCollectedLogIds.Add(logId);

        if (isTextLog == true && _textCollectedLogIds.Contains(logId) == false)
        {
            _textCollectedLogIds.Add(logId);
        }

        return true;
    }
}