using UnityEngine;

public class SubJumpScareSelectionResult
{
    public bool IsSuccess;
    public string FailReason;
    public ESubJumpScareTriggerType TriggerType;
    public ESubJumpScareType SelectedType;
    public ESubJumpScareIntensity SelectedIntensity;
    public string SelectedId;
    public string SelectedDisplayName;
    public Object SelectedAsset;

    public static SubJumpScareSelectionResult CreateFail(ESubJumpScareTriggerType triggerType, string reason)
    {
        return new SubJumpScareSelectionResult
        {
            IsSuccess = false,
            TriggerType = triggerType,
            SelectedType = ESubJumpScareType.None,
            SelectedIntensity = ESubJumpScareIntensity.None,
            FailReason = reason
        };
    }

    public static SubJumpScareSelectionResult CreateSuccess(
        ESubJumpScareTriggerType triggerType,
        ESubJumpScareType selectedType,
        ESubJumpScareIntensity selectedIntensity,
        string selectedId,
        string selectedDisplayName,
        Object selectedAsset)
    {
        return new SubJumpScareSelectionResult
        {
            IsSuccess = true,
            TriggerType = triggerType,
            SelectedType = selectedType,
            SelectedIntensity = selectedIntensity,
            SelectedId = selectedId,
            SelectedDisplayName = selectedDisplayName,
            SelectedAsset = selectedAsset
        };
    }
}