using System;
using UnityEngine;

[Serializable]
public class AchievementState
{
    [SerializeField] private AchievementId _id = AchievementId.None;
    [SerializeField] private int _currentValue = 0;
    [SerializeField] private bool _isUnlocked = false;

    public AchievementId Id => _id;
    public int CurrentValue => _currentValue;
    public bool IsUnlocked => _isUnlocked;

    public AchievementState(AchievementId id)
    {
        _id = id;
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
