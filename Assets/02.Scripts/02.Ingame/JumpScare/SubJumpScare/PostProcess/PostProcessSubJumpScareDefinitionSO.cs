using Sirenix.OdinInspector;
using UnityEngine;

public enum EPostProcessEffectType
{
    None = 0,
    Darkness = 1,
    Grayscale = 2,
    Noise = 3
}

[CreateAssetMenu(fileName = "PostProcessSubJumpScareDefinition", menuName = "Ater/JumpScare/Sub/PostProcess Definition")]
[InfoBox("포스트 프로세스 기반 서브 점프스케어 정의입니다. 화면 암전, 노이즈, 흑백화 등의 시각 효과를 제어합니다.")]
public class PostProcessSubJumpScareDefinitionSO : ScriptableObject
{
    [Title("Common")]
    [InlineProperty]
    [HideLabel]
    [PropertySpace(8f)]
    public SubJumpScareCommonData Common = new SubJumpScareCommonData();

    [Title("Post Process")]
    [BoxGroup("Settings")]
    [PropertyTooltip("적용할 포스트 프로세스 효과 종류입니다.")]
    public EPostProcessEffectType EffectType = EPostProcessEffectType.None;

    [BoxGroup("Settings")]
    [MinValue(0f)]
    [PropertyTooltip("효과의 강도입니다. 값이 클수록 더 강한 연출을 의도합니다.")]
    public float EffectStrength = 0.5f;

    [BoxGroup("Timing")]
    [MinValue(0f)]
    [PropertyTooltip("효과가 유지되는 총 시간입니다.")]
    public float Duration = 2f;

    [BoxGroup("Timing")]
    [MinValue(0f)]
    [PropertyTooltip("효과가 서서히 적용되는 시간입니다.")]
    public float FadeInTime = 0.15f;

    [BoxGroup("Timing")]
    [MinValue(0f)]
    [PropertyTooltip("효과가 서서히 사라지는 시간입니다.")]
    public float FadeOutTime = 0.25f;

    private void OnValidate()
    {
        Common.Type = ESubJumpScareType.PostProcess;
        Common.BlockSameItemAsPrevious = true;
    }
}