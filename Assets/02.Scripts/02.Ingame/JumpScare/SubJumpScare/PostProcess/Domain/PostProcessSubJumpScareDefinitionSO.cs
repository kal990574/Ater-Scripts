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
public class PostProcessSubJumpScareDefinitionSO : SubJumpScareDefinitionSOBase
{
    [Title("Common")]
    [InlineProperty]
    [HideLabel]
    [PropertySpace(8f)]
    public SubJumpScareCommonData common = new SubJumpScareCommonData();

    public override SubJumpScareCommonData Common
    {
        get
        {
            return common;
        }
    }

    [Title("Post Process")]
    [BoxGroup("Settings")]
    [PropertyTooltip("적용할 포스트 프로세스 효과 종류입니다.")]
    public EPostProcessEffectType EffectType = EPostProcessEffectType.None;

    [BoxGroup("Settings")]
    [MinValue(0f)]
    [PropertyTooltip("암전/노이즈에서 사용할 효과 강도입니다. 흑백은 사용하지 않습니다.")]
    public float EffectStrength = 0.5f;

    [BoxGroup("Timing")]
    [MinValue(0f)]
    [PropertyTooltip("효과가 유지되는 총 시간입니다.")]
    public float Duration = 2f;

    [BoxGroup("Sound")]
    [PropertyTooltip("효과가 나타날때 들리는 SFX")]
    public SoundKeyReference Sound;
    [BoxGroup("Sound")]
    [PropertyTooltip("SFX의 볼륨 0~1")]
    [MinValue(0f),MaxValue(1)]
    public float SoundVolume = 1;
    private void OnValidate()
    {
        common = EnsureCommon(common);

        common.Type = ESubJumpScareType.PostProcess;
        common.BlockSameItemAsPrevious = true;

        Duration = Mathf.Max(0.0f, Duration);
        EffectStrength = Mathf.Max(0.0f, EffectStrength);
    }
}