public class JumpScareExecutionGate
{
    public bool CanExecute(JumpScareRuntimeContext context, SubJumpScareDefinitionSO definition)
    {
        if (definition == null)
        {
            return false;
        }

        if (definition.IsInTensionRange(context.CurrentTension) == false)
        {
            return false;
        }

        if (definition.IsAllowedMode(context.PlayerMode) == false)
        {
            return false;
        }

        if (definition.BlockWhenMainJumpScareRunning == true &&
            context.IsMainJumpScareRunning == true)
        {
            return false;
        }

        if (definition.BlockWhenSubJumpScareRunning == true &&
            context.IsSubJumpScareRunning == true)
        {
            return false;
        }

        return true;
    }
}