public readonly struct UseInteractionOutcome
{
    public static UseInteractionOutcome Success(string reason = "")
    {
        return new UseInteractionOutcome(EUseInteractResult.Success, reason);
    }

    public static UseInteractionOutcome Fail(EUseInteractResult result, string reason)
    {
        return new UseInteractionOutcome(result, reason);
    }

    public UseInteractionOutcome(EUseInteractResult result, string reason)
    {
        Result = result;
        Reason = reason ?? string.Empty;
    }

    public EUseInteractResult Result { get; }
    public string Reason { get; }
    public bool IsSuccess => Result == EUseInteractResult.Success;
}
