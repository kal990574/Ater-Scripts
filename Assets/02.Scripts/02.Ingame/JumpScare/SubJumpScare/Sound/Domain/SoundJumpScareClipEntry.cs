using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public struct SoundJumpScareClipEntry
{
    [HorizontalGroup("Row", Width = 0.8f)]
    [LabelText("Sound Key")]
    public SoundKeyReference Key;

    [HorizontalGroup("Row", Width = 0.2f)]
    [MinValue(1)]
    [LabelText("Weight")]
    public int Weight;

    public bool IsValid()
    {
        return Key.IsEmpty == false
               && Weight > 0;
    }
}