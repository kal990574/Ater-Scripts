public readonly struct UI_AchievementViewData
{
    public AchievementId Id { get; }
    public string DisplayTitle { get; }
    public string DisplayDescription { get; }
    public bool IsUnlocked { get; }
    public bool IsVisible { get; }
    public int CurrentValue { get; }
    public int TargetValue { get; }
    public AchievementCategory Category { get; }

    public UI_AchievementViewData(
        AchievementId id,
        string displayTitle,
        string displayDescription,
        bool isUnlocked,
        bool isVisible,
        int currentValue,
        int targetValue,
        AchievementCategory category)
    {
        Id = id;
        DisplayTitle = displayTitle;
        DisplayDescription = displayDescription;
        IsUnlocked = isUnlocked;
        IsVisible = isVisible;
        CurrentValue = currentValue;
        TargetValue = targetValue;
        Category = category;
    }
}
