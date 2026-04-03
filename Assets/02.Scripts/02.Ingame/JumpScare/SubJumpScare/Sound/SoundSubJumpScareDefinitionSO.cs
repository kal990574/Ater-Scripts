using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundSubJumpScareDefinition", menuName = "Ater/JumpScare/Sub/Sound Definition")]
[InfoBox("사운드 기반 서브 점프스케어 정의입니다. 소리의 종류, 발생 방향, 플레이어와의 상대 거리 등을 설정합니다.")]
public class SoundSubJumpScareDefinitionSO : ScriptableObject
{
    [Title("Common")]
    [InlineProperty]
    [HideLabel]
    [PropertySpace(8f)]
    public SubJumpScareCommonData Common = new SubJumpScareCommonData();

    [Title("Sound")]
    [BoxGroup("Settings")]
    [PropertyTooltip("재생할 점프스케어 사운드 클립입니다.")]
    [PreviewField(70, ObjectFieldAlignment.Left)]
    public AudioClip[] Clip;

    [BoxGroup("Settings")]
    [PropertyTooltip("플레이어 기준 어느 방향에서 사운드가 들릴지 결정합니다. Random이면 임의 방향을 사용합니다.")]
    public ERelativeDirection RelativeDirection = ERelativeDirection.Random;

    [BoxGroup("Settings")]
    [MinValue(0f)]
    [PropertyTooltip("플레이어로부터 사운드가 발생할 상대 거리입니다.")]
    public float Distance = 8f;

    private void OnValidate()
    {
        Common.Type = ESubJumpScareType.Sound;
        Common.BlockSameIntensityAsPrevious = true;
        Common.BlockSameItemAsPrevious = true;
    }
}