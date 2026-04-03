using System.Collections.Generic;
using UnityEngine;

public readonly struct FakeEnemyJumpScareExecuteRequest
{
    public Vector3 CameraPosition { get; }
    public Vector3 CameraForward{ get; }
    public Transform PlayerTransform{ get; }

    public float MinDistance{ get; }
    public float MaxDistance{ get; }
    public float AllowedForwardAngle{ get; }
    
    public IReadOnlyList<GameObject> PosePrefabs{ get; }

    public bool UseDeterministicSelection{ get; }
    public int DeterministicSeed{ get; }

    public FakeEnemyJumpScareExecuteRequest(
        Vector3 cameraPosition,
        Vector3 cameraForward,
        Transform playerTransform,
        float minDistance,
        float maxDistance,
        float allowedForwardAngle,
        IReadOnlyList<GameObject> posePrefabs,
        bool useDeterministicSelection,
        int deterministicSeed)
    {
        CameraPosition = cameraPosition;
        CameraForward = cameraForward;
        PlayerTransform = playerTransform;
        MinDistance = minDistance;
        MaxDistance = maxDistance;
        AllowedForwardAngle = allowedForwardAngle;
        PosePrefabs = posePrefabs;
        UseDeterministicSelection = useDeterministicSelection;
        DeterministicSeed = deterministicSeed;
    }

    public bool IsValid()
    {
        if (MinDistance < 0.0f)
        {
            return false;
        }

        if (MaxDistance < MinDistance)
        {
            return false;
        }

        if (AllowedForwardAngle < 0.0f)
        {
            return false;
        }

        if (PosePrefabs == null || PosePrefabs.Count == 0)
        {
            return false;
        }

        return true;
    }
}