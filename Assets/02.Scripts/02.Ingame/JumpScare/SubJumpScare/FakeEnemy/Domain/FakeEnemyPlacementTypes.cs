using System;
using System.Collections.Generic;
using UnityEngine;

public enum EFakeEnemyPlacementCandidateState
{
    None = 0,
    GroundMiss = 1,
    InvalidGroundTag = 2,
    InvalidDistance = 3,
    OverlapBlocked = 4,
    Occluded = 5,
    Valid = 6,
    Selected = 7
}

public enum EFakeEnemyPlacementFailReason
{
    None = 0,
    InvalidForward = 1,
    NoValidCandidate = 2
}

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

[Serializable]
public struct FakeEnemyPlacementResult
{
    public bool Success;
    public Vector3 Position;
    public Quaternion Rotation;
    public EFakeEnemyPlacementFailReason FailReason;
    public int SelectedCandidateIndex;

    public static FakeEnemyPlacementResult CreateFailure(EFakeEnemyPlacementFailReason failReason)
    {
        FakeEnemyPlacementResult result = new FakeEnemyPlacementResult();
        result.Success = false;
        result.Position = Vector3.zero;
        result.Rotation = Quaternion.identity;
        result.FailReason = failReason;
        result.SelectedCandidateIndex = -1;
        return result;
    }

    public static FakeEnemyPlacementResult CreateSuccess(
        Vector3 position,
        Quaternion rotation,
        int selectedCandidateIndex)
    {
        FakeEnemyPlacementResult result = new FakeEnemyPlacementResult();
        result.Success = true;
        result.Position = position;
        result.Rotation = rotation;
        result.FailReason = EFakeEnemyPlacementFailReason.None;
        result.SelectedCandidateIndex = selectedCandidateIndex;
        return result;
    }
}

[Serializable]
public struct FakeEnemyPlacementCandidateDebugInfo
{
    public Vector3 SamplePoint;
    public Vector3 GroundedPoint;
    public EFakeEnemyPlacementCandidateState State;

    public FakeEnemyPlacementCandidateDebugInfo(
        Vector3 samplePoint,
        Vector3 groundedPoint,
        EFakeEnemyPlacementCandidateState state)
    {
        SamplePoint = samplePoint;
        GroundedPoint = groundedPoint;
        State = state;
    }
}

[Serializable]
public sealed class FakeEnemyPlacementDebugSnapshot
{
    public List<FakeEnemyPlacementCandidateDebugInfo> Candidates { get; private set; }
    public FakeEnemyPlacementResult Result { get; private set; }

    public FakeEnemyPlacementDebugSnapshot()
    {
        Candidates = new List<FakeEnemyPlacementCandidateDebugInfo>();
        Result = FakeEnemyPlacementResult.CreateFailure(EFakeEnemyPlacementFailReason.None);
    }

    public void Clear()
    {
        Candidates.Clear();
        Result = FakeEnemyPlacementResult.CreateFailure(EFakeEnemyPlacementFailReason.None);
    }

    public void SetResult(FakeEnemyPlacementResult result)
    {
        Result = result;
    }
}