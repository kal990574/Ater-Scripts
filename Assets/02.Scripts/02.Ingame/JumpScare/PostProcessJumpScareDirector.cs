using UnityEngine;

public class PostProcessJumpScareDirector : BaseSubJumpScareDirector
{
    public override ESubJumpScareType Type => ESubJumpScareType.PostProcess;

    public override bool CanExecute(JumpScareRuntimeContext context, SubJumpScareDefinitionSO definition)
    {
        PostProcessJumpScareDefinitionSO postProcessDefinition =
            GetDefinition<PostProcessJumpScareDefinitionSO>(definition);

        if (postProcessDefinition == null)
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
        PostProcessJumpScareDefinitionSO postProcessDefinition =
            GetDefinition<PostProcessJumpScareDefinitionSO>(definition);

        if (postProcessDefinition == null)
        {
            return null;
        }

        SubJumpScareHandle handle = new SubJumpScareHandle(
            postProcessDefinition.Id,
            postProcessDefinition.Type,
            intensity);

        float tensionDelta = -postProcessDefinition.GetTensionDecrease(intensity);

        SubJumpScareExecutionResult result = new SubJumpScareExecutionResult(
            ESubJumpScareResult.Success,
            tensionDelta,
            "PostProcessJumpScarePlayed");

        handle.Complete(result);
        return handle;
    }
}