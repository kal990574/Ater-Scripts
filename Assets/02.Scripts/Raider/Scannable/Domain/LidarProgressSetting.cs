using System;
using UnityEngine;

[Serializable]
public class LidarProgressSetting
{
    [Header("Progress")]
    [Min(0.01f)] public float RequiredScanTime = 5.0f;
    [Min(0f)] public float ReturnSpeed = 2.0f;

    [Header("Minigame")]
    [Min(0f)] public float MinMinigameInterval = 1.5f;
    [Min(0f)] public float MaxMinigameInterval = 4.0f;
    [Min(0f)] public float FailPenalty = 1.0f;
    [Min(0f)] public float GreatSuccessBonus = 1.0f;
    [Min(0f)] public float MinigameTriggerInterval = 2.0f;
}
