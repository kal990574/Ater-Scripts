using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
[HideReferenceObjectPicker]
public class AchievementDefinition
{
    [FoldoutGroup("$FoldoutTitle", expanded: false)]
    [LabelText("ID")]
    [SerializeField]
    private AchievementKeyReference _id;
    
    [FoldoutGroup("$FoldoutTitle")]
    [LabelText("아이콘")]
    [SerializeField]
    private Sprite _icon;

    [FoldoutGroup("$FoldoutTitle")]
    [LabelText("제목")]
    [SerializeField]
    private string _title = string.Empty;

    [FoldoutGroup("$FoldoutTitle")]
    [LabelText("설명")]
    [TextArea(2, 4)]
    [SerializeField]
    private string _description = string.Empty;

    [FoldoutGroup("$FoldoutTitle")]
    [LabelText("카테고리")]
    [SerializeField]
    private EAchievementCategory _category;

    [FoldoutGroup("$FoldoutTitle")]
    [LabelText("목표값")]
    [MinValue(1)]
    [SerializeField]
    private int _targetValue = 1;

    [FoldoutGroup("$FoldoutTitle")]
    [LabelText("표시 방식")]
    [SerializeField]
    private EAchievementVisibilityType _visibilityType = EAchievementVisibilityType.Normal;

    public string Id => _id.Value;
    public Sprite Icon => _icon;
    public string Title => _title;
    public string Description => _description;
    public EAchievementCategory Category => _category;
    public int TargetValue => _targetValue;
    public EAchievementVisibilityType VisibilityType => _visibilityType;

    private string FoldoutTitle => string.IsNullOrWhiteSpace(_title) == false ? _title : "Achievement";
}
