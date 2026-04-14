public readonly struct UI_AchievementViewData
{
    public string Id { get; }
    public string DisplayTitle { get; }
    public string DisplayDescription { get; }
    public string UnlockStatusText { get; }
    public bool IsUnlocked { get; }
    public bool IsVisible { get; }
    public int CurrentValue { get; }
    public int TargetValue { get; }
    public EAchievementCategory Category { get; }

    public UI_AchievementViewData(
        string id,
        string displayTitle,
        string displayDescription,
        string unlockStatusText,
        bool isUnlocked,
        bool isVisible,
        int currentValue,
        int targetValue,
        EAchievementCategory category)
    {
        Id = id;
        DisplayTitle = displayTitle;
        DisplayDescription = displayDescription;
        UnlockStatusText = unlockStatusText;
        IsUnlocked = isUnlocked;
        IsVisible = isVisible;
        CurrentValue = currentValue;
        TargetValue = targetValue;
        Category = category;
    }
}
