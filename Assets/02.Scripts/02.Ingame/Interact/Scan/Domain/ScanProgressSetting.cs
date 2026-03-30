using System;
using UnityEngine;

[Serializable]
public class ScanProgressSetting
{
    [Header("Progress")]
    [Min(0.01f)] public float RequiredScanTime = 5.0f;
    [Min(0f)] public float ReturnSpeed = 2.0f;
}
