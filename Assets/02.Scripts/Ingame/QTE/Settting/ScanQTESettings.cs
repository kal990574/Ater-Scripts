using System;
using UnityEngine;

[Serializable]
public class ScanQTESettings
{
    [Min(0f)] public float MinInterval = 1.5f;
    [Min(0f)] public float MaxInterval = 4.0f;
    [Min(0f)] public float FailPenalty = 1.0f;
    [Min(0f)] public float GreatSuccessBonus = 1.0f;
}
