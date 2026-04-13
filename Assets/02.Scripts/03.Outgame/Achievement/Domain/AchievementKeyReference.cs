using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public static class AchievementKeyDropdown
{
    public static IEnumerable<string> GetValues()
    {
        return StaticStringDropdown<AchievementKey>.GetValues();
    }

    public static bool Contains(string value)
    {
        return StaticStringDropdown<AchievementKey>.Contains(value);
    }

    public static IEnumerable<ValueDropdownItem<string>> GetItems()
    {
        return StaticStringDropdown<AchievementKey>.GetItems();
    }
}

[Serializable]
public struct AchievementKeyReference
{
    [ValueDropdown("@AchievementKeyDropdown.GetItems()")]
    public string Value;

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);
    public bool IsValid => string.IsNullOrEmpty(Value) || AchievementKeyDropdown.Contains(Value);

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(AchievementKeyReference reference)
    {
        return reference.Value;
    }
}
