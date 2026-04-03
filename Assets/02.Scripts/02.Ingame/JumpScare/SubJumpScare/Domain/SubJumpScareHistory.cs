using System;

/// <summary>
/// 이전에 어떤 점프스케어가 발동했는지 기록
/// </summary>
public class SubJumpScareHistory
{
    public string LastSelectedId { get; private set; }
    public ESubJumpScareType LastSelectedType { get; private set; }
    public ESubJumpScareIntensity LastSelectedIntensity { get; private set; }

    public SubJumpScareHistory()
    {
        Clear();
    }

    public void Record(SubJumpScareSelectionResult result)
    {
        if (result.IsSuccess == false)
        {
            return;
        }

        LastSelectedId = result.Data.Id;
        LastSelectedType = result.Data.Type;
        LastSelectedIntensity = result.Data.Intensity;
    }

    public bool IsSameItem(string itemId)
    {
        return string.Equals(LastSelectedId, itemId, StringComparison.Ordinal);
    }

    public bool IsSameIntensity(ESubJumpScareIntensity intensity)
    {
        return LastSelectedIntensity == intensity;
    }

    public void Clear()
    {
        LastSelectedId = string.Empty;
        LastSelectedType = ESubJumpScareType.None;
        LastSelectedIntensity = ESubJumpScareIntensity.None;
    }
}