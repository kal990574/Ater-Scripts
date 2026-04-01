using UnityEngine;

public class SoundJumpScareDirector : BaseSubJumpScareDirector
{
    public override ESubJumpScareType Type => ESubJumpScareType.Sound;

    public override bool CanExecute(JumpScareRuntimeContext context, SubJumpScareDefinitionSO definition)
    {
        SoundJumpScareDefinitionSO soundDefinition =
            GetDefinition<SoundJumpScareDefinitionSO>(definition);

        if (soundDefinition == null)
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
        SoundJumpScareDefinitionSO soundDefinition =
            GetDefinition<SoundJumpScareDefinitionSO>(definition);

        if (soundDefinition == null)
        {
            return null;
        }

        SubJumpScareHandle handle = new SubJumpScareHandle(
            soundDefinition.Id,
            soundDefinition.Type,
            intensity);

        float tensionDelta = -soundDefinition.GetTensionDecrease(intensity);

        SubJumpScareExecutionResult result = new SubJumpScareExecutionResult(
            ESubJumpScareResult.Success,
            tensionDelta,
            "SoundJumpScarePlayed");

        handle.Complete(result);
        return handle;
    }
}