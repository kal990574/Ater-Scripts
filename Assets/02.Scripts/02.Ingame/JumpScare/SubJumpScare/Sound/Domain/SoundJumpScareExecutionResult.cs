public readonly struct SoundJumpScareExecutionResult
{
    public readonly bool IsSuccess;
    public readonly string Reason;
    public readonly SoundJumpScareResolvedData ResolvedData;

    private SoundJumpScareExecutionResult(
        bool isSuccess,
        string reason,
        SoundJumpScareResolvedData resolvedData)
    {
        IsSuccess = isSuccess;
        Reason = reason;
        ResolvedData = resolvedData;
    }

    public static SoundJumpScareExecutionResult CreateSuccess(SoundJumpScareResolvedData resolvedData)
    {
        return new SoundJumpScareExecutionResult(true, string.Empty, resolvedData);
    }

    public static SoundJumpScareExecutionResult CreateFailure(string reason)
    {
        return new SoundJumpScareExecutionResult(false, reason, default);
    }
}