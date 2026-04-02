using System;

public readonly struct SubJumpScareSelectionResult
{
    public bool IsSuccess { get; }
    public string FailReason { get; }
    public ESubJumpScareTriggerType TriggerType { get; }
    public SubJumpScareCommonData Data { get; }

    private SubJumpScareSelectionResult(
        bool isSuccess,
        ESubJumpScareTriggerType triggerType,
        string failReason,
        SubJumpScareCommonData data)
    {
        IsSuccess = isSuccess;
        TriggerType = triggerType;
        FailReason = failReason;
        Data = data;
    }

    public static SubJumpScareSelectionResult CreateFail(
        ESubJumpScareTriggerType triggerType,
        string reason)
    {
        return new SubJumpScareSelectionResult(
            false,
            triggerType,
            reason,
            null);
    }

    public static SubJumpScareSelectionResult CreateSuccess(
        ESubJumpScareTriggerType triggerType,
        SubJumpScareCommonData data)
    {
        return new SubJumpScareSelectionResult(
            true,
            triggerType,
            null,
            data);
    }
}