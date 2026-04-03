using System;
using System.Collections.Generic;

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