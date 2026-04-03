using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
[InlineProperty]
[HideLabel]
public class SubJumpScareCommonData
{
    [FoldoutGroup("Identity", expanded: true)]
    [PropertyTooltip("이 점프스케어 데이터를 식별하기 위한 고유 ID입니다. 런타임 선택, 로그, 추적에 사용됩니다.")]
    [ValidateInput(nameof(ValidateId), "Id는 비어 있을 수 없습니다.")]
    public string Id;

    [FoldoutGroup("Identity", expanded: true)]
    [PropertyTooltip("인스펙터, 디버그 로그, 기획 확인용 표시 이름입니다.")]
    [ValidateInput(nameof(ValidateDisplayName), "DisplayName은 비어 있을 수 없습니다.")]
    public string DisplayName;

    [FoldoutGroup("Common", expanded: true)]
    [ReadOnly]
    [PropertyTooltip("이 정의의 점프스케어 타입입니다. 각 SO의 OnValidate()에서 자동으로 설정됩니다.")]
    public ESubJumpScareType Type = ESubJumpScareType.None;

    [FoldoutGroup("Common", expanded: true)]
    [PropertyTooltip("현재 정의의 강도입니다. 약/중/강 등 연출 강도 구분에 사용합니다.")]
    public ESubJumpScareIntensity Intensity = ESubJumpScareIntensity.Weak;

    [FoldoutGroup("Common", expanded: true)]
    [MinValue(1)]
    [PropertyTooltip("동일 조건에서 후보가 여러 개일 때 선택 확률에 영향을 주는 가중치입니다. 높을수록 뽑힐 가능성이 커집니다.")]
    public int Weight = 1;

    [FoldoutGroup("Tension Range", expanded: true)]
    [PropertyTooltip("이 정의가 선택될 수 있는 최소 긴장도입니다. totalTension이 이 값 이상이어야 합니다.")]
    public float MinTension = 0f;

    [FoldoutGroup("Tension Range", expanded: true)]
    [PropertyTooltip("이 정의가 선택될 수 있는 최대 긴장도입니다. totalTension이 이 값 미만이어야 합니다.")]
    [ValidateInput(nameof(ValidateMaxTension), "MaxTension은 MinTension보다 커야 합니다.")]
    public float MaxTension = 100f;

    [FoldoutGroup("Cooldown", expanded: true)]
    [MinValue(0f)]
    [PropertyTooltip("같은 타입의 점프스케어가 다시 발동되기 전까지 필요한 최소 대기시간입니다.")]
    public float TypeCooldown = 3f;

    [FoldoutGroup("Cooldown", expanded: true)]
    [MinValue(0f)]
    [PropertyTooltip("같은 아이템(같은 정의 데이터)이 다시 발동되기 전까지 필요한 최소 대기시간입니다.")]
    public float ItemCooldown = 5f;

    [FoldoutGroup("Cooldown", expanded: true)]
    [MinValue(0f)]
    [PropertyTooltip("이 점프스케어가 글로벌 쿨다운에 더할 값입니다. 전체 연출 빈도를 제어하는 데 사용합니다.")]
    public float GlobalCooldownContribution = 2f;

    [FoldoutGroup("Repeat Rules", expanded: true)]
    [PropertyTooltip("직전에 발동한 점프스케어와 동일한 강도라면 이번 후보를 제외할지 여부입니다.")]
    public bool BlockSameIntensityAsPrevious = false;

    [FoldoutGroup("Repeat Rules", expanded: true)]
    [PropertyTooltip("직전에 발동한 점프스케어와 동일한 데이터 항목이라면 이번 후보를 제외할지 여부입니다.")]
    public bool BlockSameItemAsPrevious = false;

    [FoldoutGroup("Tension Feedback", expanded: true)]
    [MinValue(0f)]
    [PropertyTooltip("이 점프스케어가 실제 발생했을 때 TotalTension에서 감소시킬 양입니다.")]
    public float TensionDecreaseOnTriggered = 0f;

    public bool IsValid()
    {
        return string.IsNullOrWhiteSpace(Id) == false
               && string.IsNullOrWhiteSpace(DisplayName) == false
               && Type != ESubJumpScareType.None
               && Intensity != ESubJumpScareIntensity.None
               && Weight > 0
               && MaxTension > MinTension;
    }

    public bool IsInTensionRange(float totalTension)
    {
        return totalTension >= MinTension && totalTension < MaxTension;
    }

    private bool ValidateId(string value)
    {
        return string.IsNullOrWhiteSpace(value) == false;
    }

    private bool ValidateDisplayName(string value)
    {
        return string.IsNullOrWhiteSpace(value) == false;
    }

    private bool ValidateMaxTension(float value)
    {
        return value > MinTension;
    }
}