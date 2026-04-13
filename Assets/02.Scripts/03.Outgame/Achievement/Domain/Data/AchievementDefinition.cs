using System;
using UnityEngine;

[Serializable]
public class AchievementDefinition
{
    [SerializeField] private AchievementKeyReference _id;
    [SerializeField] private string _title = string.Empty;
    [SerializeField] [TextArea(2, 4)] private string _description = string.Empty;
    [SerializeField] private EAchievementCategory _category = EAchievementCategory.Mechanic;
    [SerializeField] private EAchievementVisibilityType _visibilityType = EAchievementVisibilityType.Normal;
    [SerializeField] private int _targetValue = 1;

    public string Id => _id;
    public string Title => _title;
    public string Description => _description;
    public EAchievementCategory Category => _category;
    public EAchievementVisibilityType VisibilityType => _visibilityType;
    public int TargetValue => Mathf.Max(1, _targetValue);
}
