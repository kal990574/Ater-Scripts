using System;
using UnityEngine;

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