public class SubJumpScareHandle
{
    public string DefinitionId { get; }
    public ESubJumpScareType Type { get; }
    public EJumpScareIntensity Intensity { get; }

    public bool IsCompleted { get; private set; }
    public SubJumpScareExecutionResult Result { get; private set; }

    public SubJumpScareHandle(
        string definitionId,
        ESubJumpScareType type,
        EJumpScareIntensity intensity)
    {
        DefinitionId = definitionId;
        Type = type;
        Intensity = intensity;
        IsCompleted = false;
        Result = new SubJumpScareExecutionResult(
            ESubJumpScareResult.None,
            0.0f,
            string.Empty);
    }

    public void Complete(SubJumpScareExecutionResult result)
    {
        if (IsCompleted == true)
        {
            return;
        }

        Result = result;
        IsCompleted = true;
    }
}