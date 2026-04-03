using System;
using UnityEngine;

[Serializable]
public struct FakeEnemyPlacementRequest
{
    public Vector3 CameraPosition;
    public Vector3 CameraForward;
    public Vector3 PlayerPosition;
    public float MinDistance;
    public float MaxDistance;
    public bool UseDeterministicSelection;
    public int DeterministicSeed;

    public FakeEnemyPlacementRequest(
        Vector3 cameraPosition,
        Vector3 cameraForward,
        Vector3 playerPosition,
        float minDistance,
        float maxDistance,
        bool useDeterministicSelection,
        int deterministicSeed)
    {
        CameraPosition = cameraPosition;
        CameraForward = cameraForward;
        PlayerPosition = playerPosition;
        MinDistance = minDistance;
        MaxDistance = maxDistance;
        UseDeterministicSelection = useDeterministicSelection;
        DeterministicSeed = deterministicSeed;
    }
}