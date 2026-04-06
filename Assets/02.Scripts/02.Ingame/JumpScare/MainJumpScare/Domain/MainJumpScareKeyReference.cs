using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;

public class MainJumpScareKeyDropDown
{
    public static IEnumerable<string> GetValues()
    {
        return StaticStringDropdown<MainJumpScareKey>.GetValues();
    }

    public static bool Contains(string value)
    {
        return StaticStringDropdown<MainJumpScareKey>.Contains(value);
    }

    public static IEnumerable<ValueDropdownItem<string>> GetItems()
    {
        return StaticStringDropdown<MainJumpScareKey>.GetItems();
    }
}

[Serializable]
public struct MainJumpScareKeyReference
{
    [ValueDropdown("@MainJumpScareKeyDropDown.GetItems()")]
    public string Value;
    
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);
    public bool IsValid => MainJumpScareKeyDropDown.Contains(Value);

    public override string ToString()
    {
        return Value;
    }
    
    public static implicit operator string(MainJumpScareKeyReference reference)
    {
        return reference.Value;
    }
}
