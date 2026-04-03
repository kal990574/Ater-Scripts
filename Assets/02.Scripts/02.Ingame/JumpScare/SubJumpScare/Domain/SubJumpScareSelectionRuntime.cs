using System;
using UnityEngine;

[Serializable]
public class SubJumpScareSelectionRuntime
{
    [Header("Core Runtime")]
    public float TotalTension;
    public bool IsMainJumpScareRunning;
    public bool IsInMainEndGraceTime;
    public bool IsPostProcessActive;

    [Header("Player State")]
    public EPlayerInteractMode CurrentPlayerInteractMode;

    [Header("Sonar Runtime")]
    public bool IsSonarAvailable = true;
    public bool CanPlaceFakeEnemyThisAttempt = true;

    [Header("Important Audio")]
    public bool IsImportantVoicePlaying = false;
}