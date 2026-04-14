using System;
using UnityEngine;

//업적데이터의 진행상황
[Serializable]
public class AchievementState
{
    [SerializeField] private string _id = AchievementKey.None;
    [SerializeField] private int _currentValue = 0;
    [SerializeField] private bool _isUnlocked = false;
    [SerializeField] private long _unlockedAtUnixSeconds = 0;

    public string Id => _id;
    public int CurrentValue => _currentValue;
    public bool IsUnlocked => _isUnlocked;
    public long UnlockedAtUnixSeconds => _unlockedAtUnixSeconds;

    public AchievementState(string id)
    {
        _id = string.IsNullOrWhiteSpace(id) ? AchievementKey.None : id;
        _currentValue = 0;
        _isUnlocked = false;
    }

    public void SetCurrentValue(int value)
    {
        _currentValue = Mathf.Max(0, value);
    }

    public void AddProgress(int value)
    {
        if (value <= 0)
        {
            return;
        }

        _currentValue += value;
    }

    public bool TryUnlock()
    {
        if (_isUnlocked == true)
        {
            return false;
        }

        _isUnlocked = true;
        _unlockedAtUnixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return true;
    }
    
    public void Reset()
    {
        _currentValue = 0;
        _isUnlocked = false;
        _unlockedAtUnixSeconds = 0;
    }
}
