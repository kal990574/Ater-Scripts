using System;
using UnityEngine;

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