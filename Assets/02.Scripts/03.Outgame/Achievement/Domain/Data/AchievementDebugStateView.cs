using System;
using Sirenix.OdinInspector;

[Serializable]
public class AchievementDebugStateView
{
    [TableColumnWidth(220, false)]
    [LabelText("ID")]
    public string Id;

    [TableColumnWidth(180, false)]
    [LabelText("Title")]
    public string Title;

    [TableColumnWidth(80, false)]
    [LabelText("Current")]
    public int CurrentValue;

    [TableColumnWidth(80, false)]
    [LabelText("Target")]
    public int TargetValue;

    [TableColumnWidth(80, false)]
    [LabelText("Unlocked")]
    public bool IsUnlocked;

    public AchievementDebugStateView(
        string id,
        string title,
        int currentValue,
        int targetValue,
        bool isUnlocked)
    {
        Id = id;
        Title = title;
        CurrentValue = currentValue;
        TargetValue = targetValue;
        IsUnlocked = isUnlocked;
    }
}
