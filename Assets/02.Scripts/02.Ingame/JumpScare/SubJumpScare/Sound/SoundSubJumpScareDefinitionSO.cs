using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundSubJumpScareDefinition", menuName = "Ater/JumpScare/Sub/Sound Definition")]
[InfoBox("사운드 기반 서브 점프스케어 정의입니다. 소리의 종류, 발생 방향, 플레이어와의 상대 거리 등을 설정합니다.")]
public class SoundSubJumpScareDefinitionSO : SubJumpScareDefinitionSOBase
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

    [Title("Sound")]
    [BoxGroup("Settings")]
    [PropertyTooltip("재생할 점프스케어 사운드 클립입니다.")]
    [PreviewField(70, ObjectFieldAlignment.Left)]
    public AudioClip[] Clip;

    [BoxGroup("Settings")]
    [MinValue(0f)]
    [PropertyTooltip("사운드 재생 볼륨입니다.")]
    public float Volume = 1.0f;

    private void OnValidate()
    {
        common = EnsureCommon(common);

        common.Type = ESubJumpScareType.Sound;
        common.BlockSameItemAsPrevious = true;

        Volume = Mathf.Clamp01(Volume);
    }
}