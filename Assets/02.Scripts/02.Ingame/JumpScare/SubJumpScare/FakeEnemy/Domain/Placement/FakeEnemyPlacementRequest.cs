using UnityEngine;

public struct FakeEnemyPlacementRequest
{
    public Vector3 CameraPosition;
    public Vector3 CameraForward;
    public Transform PlayerTransform;

    public float MinDistance;
    public float MaxDistance;
    public float AllowedForwardAngle;

    public bool UseDeterministicSelection;
    public int DeterministicSeed;

    public FakeEnemyPlacementRequest(
        Vector3 cameraPosition,
        Vector3 cameraForward,
        Transform playerTransform,
        float minDistance,
        float maxDistance,
        float allowedForwardAngle,
        bool useDeterministicSelection,
        int deterministicSeed)
    {
        CameraPosition = cameraPosition;
        CameraForward = cameraForward;
        PlayerTransform = playerTransform;
        MinDistance = minDistance;
        MaxDistance = maxDistance;
        AllowedForwardAngle = allowedForwardAngle;
        UseDeterministicSelection = useDeterministicSelection;
        DeterministicSeed = deterministicSeed;
    }
}