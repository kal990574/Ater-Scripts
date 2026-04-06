using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;

public static class SoundKeyDropdown
{
    public static IEnumerable<string> GetValues()
    {
        return StaticStringDropdown<SoundKey>.GetValues();
    }

    public static bool Contains(string value)
    {
        return StaticStringDropdown<SoundKey>.Contains(value);
    }

    public static IEnumerable<ValueDropdownItem<string>> GetItems()
    {
        return StaticStringDropdown<SoundKey>.GetItems();
    }
}

[Serializable]
public struct SoundKeyReference
{
    [ValueDropdown("@SoundKeyDropdown.GetItems()")]
    public string Value;

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);
    public bool IsValid => SoundKeyDropdown.Contains(Value);

    public override string ToString()
    {
        return Value;
    }
    
    public static implicit operator string(SoundKeyReference reference)
    {
        return reference.Value;
    }
}
