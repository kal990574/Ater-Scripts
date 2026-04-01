public readonly struct SubJumpScareExecutionResult
{
    public ESubJumpScareResult Result { get; }
    public float TensionDelta { get; }
    public string Reason { get; }

    public SubJumpScareExecutionResult(
        ESubJumpScareResult result,
        float tensionDelta,
        string reason)
    {
        Result = result;
        TensionDelta = tensionDelta;
        Reason = reason;
    }
}