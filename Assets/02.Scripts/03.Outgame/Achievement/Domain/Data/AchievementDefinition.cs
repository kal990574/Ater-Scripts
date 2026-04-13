using System;
using UnityEngine;

[Serializable]
public class AchievementDefinition
{
    [SerializeField] private AchievementId _id = AchievementId.None;
    [SerializeField] private string _title = string.Empty;
    [SerializeField] [TextArea(2, 4)] private string _description = string.Empty;
    [SerializeField] private AchievementCategory _category = AchievementCategory.Mechanic;
    [SerializeField] private AchievementVisibilityType _visibilityType = AchievementVisibilityType.Normal;
    [SerializeField] private AchievementMetricType _metricType = AchievementMetricType.None;
    [SerializeField] private int _targetValue = 1;

    public AchievementId Id => _id;
    public string Title => _title;
    public string Description => _description;
    public AchievementCategory Category => _category;
    public AchievementVisibilityType VisibilityType => _visibilityType;
    public AchievementMetricType MetricType => _metricType;
    public int TargetValue => Mathf.Max(1, _targetValue);
}
