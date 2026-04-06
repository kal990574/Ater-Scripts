using System;
using Sirenix.OdinInspector;

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
}
