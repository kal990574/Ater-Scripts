using System;

/// <summary>
/// 현재 프레임의 런타임 상태를 선택 로직에 전달
/// </summary>
[Serializable]
public class SubJumpScareContext
{
    public float TotalTension;
    public EPlayerInteractMode CurrentPlayerInteractMode;

    public bool IsMainJumpScareRunning;
    public bool IsInMainEndGraceTime;

    public bool IsPostProcessActive;
    public bool IsImportantVoicePlaying;

    public bool IsSonarAvailable = true;
    public bool CanPlaceFakeEnemyThisAttempt = true;
}