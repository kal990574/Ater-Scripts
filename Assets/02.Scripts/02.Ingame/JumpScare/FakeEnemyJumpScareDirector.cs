using UnityEngine;

public class FakeEnemyJumpScareDirector : BaseSubJumpScareDirector
{
    public override ESubJumpScareType Type => ESubJumpScareType.FakeEnemy;

    public override bool CanExecute(JumpScareRuntimeContext context, SubJumpScareDefinitionSO definition)
    {
        FakeEnemyJumpScareDefinitionSO fakeEnemyDefinition =
            GetDefinition<FakeEnemyJumpScareDefinitionSO>(definition);

        if (fakeEnemyDefinition == null)
        {
            return false;
        }

        return true;
    }

    public override SubJumpScareHandle Execute(
        JumpScareRuntimeContext context,
        SubJumpScareDefinitionSO definition,
        EJumpScareIntensity intensity)
    {
        FakeEnemyJumpScareDefinitionSO fakeEnemyDefinition =
            GetDefinition<FakeEnemyJumpScareDefinitionSO>(definition);

        if (fakeEnemyDefinition == null)
        {
            return null;
        }

        SubJumpScareHandle handle = new SubJumpScareHandle(
            fakeEnemyDefinition.Id,
            fakeEnemyDefinition.Type,
            intensity);

        float tensionDelta = -fakeEnemyDefinition.GetSuccessTensionDecrease(intensity);

        SubJumpScareExecutionResult result = new SubJumpScareExecutionResult(
            ESubJumpScareResult.Success,
            tensionDelta,
            "FakeEnemyDetected");

        handle.Complete(result);
        return handle;
    }
}