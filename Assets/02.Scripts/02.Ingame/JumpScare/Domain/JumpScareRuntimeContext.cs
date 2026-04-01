using UnityEngine;

public readonly struct JumpScareRuntimeContext
{
    public float CurrentTension { get; }
    public Vector3 PlayerPosition { get; }
    public Vector3 PlayerForward { get; }
    public EPlayerInteractMode PlayerMode { get; }
    public bool IsMainJumpScareRunning { get; }
    public bool IsSubJumpScareRunning { get; }
    public string CurrentZoneId { get; }

    public JumpScareRuntimeContext(
        float currentTension,
        Vector3 playerPosition,
        Vector3 playerForward,
        EPlayerInteractMode playerMode,
        bool isMainJumpScareRunning,
        bool isSubJumpScareRunning,
        string currentZoneId)
    {
        CurrentTension = currentTension;
        PlayerPosition = playerPosition;
        PlayerForward = playerForward;
        PlayerMode = playerMode;
        IsMainJumpScareRunning = isMainJumpScareRunning;
        IsSubJumpScareRunning = isSubJumpScareRunning;
        CurrentZoneId = currentZoneId;
    }
}