using System;

[Serializable]
public sealed class SubJumpScareItemCooldownDebugInfo
{
    public string ItemId;
    public float RemainingTime;

    public SubJumpScareItemCooldownDebugInfo(string itemId, float remainingTime)
    {
        ItemId = itemId;
        RemainingTime = remainingTime;
    }
}
