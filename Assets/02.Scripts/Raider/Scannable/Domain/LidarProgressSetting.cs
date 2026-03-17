using System;
using UnityEngine;

[Serializable]
public class LidarProgressSetting
{
    public float RequiredScanTime =5.0f;
    public float ReturnSpeed=2.0f;
    public float FailPenalty =1.0f;
    public float GreatSuccessBonus = 1.0f;
    public float MinigameTriggerInterval = 2.0f;

    public float MinMinigameInterval = 1.5f;
    public float MaxMinigameInterval = 4.0f;
}