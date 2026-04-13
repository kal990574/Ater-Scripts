using System;
using UnityEngine;

//업적의 진행상태 데이터. id와 현재 값. 달성여부를 가진다.
[Serializable]
public class AchievementState
{
    [SerializeField] private string _id = AchievementKey.None;
    [SerializeField] private int _currentValue = 0;
    [SerializeField] private bool _isUnlocked = false;

    public string Id => _id;
    public int CurrentValue => _currentValue;
    public bool IsUnlocked => _isUnlocked;

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
        return true;
    }
}
