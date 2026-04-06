using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundSubJumpScareDefinition", menuName = "Ater/JumpScare/Sub/Sound Definition")]
[InfoBox("사운드 기반 서브 점프스케어 정의입니다. 강도 + 테마 단위로 만들고, 내부에 동일 계열 사운드 변주를 등록합니다.")]
public class SoundSubJumpScareDefinitionSO : SubJumpScareDefinitionSOBase
{
    [Title("Common")]
    [InlineProperty]
    [HideLabel]
    [PropertySpace(8f)]
    public SubJumpScareCommonData CommonData = new SubJumpScareCommonData();

    public override SubJumpScareCommonData Common
    {
        get
        {
            return CommonData;
        }
    }

    [Title("Sound")]
    [BoxGroup("Sound/Playback")]
    [PropertyTooltip("같은 정의 내부에서 랜덤 선택될 사운드 키 목록입니다.")]
    [ListDrawerSettings(Expanded = true)]
    public List<SoundJumpScareClipEntry> SoundEntries = new List<SoundJumpScareClipEntry>();

    [BoxGroup("Sound/Playback")]
    [MinValue(0f)]
    [MaxValue(1f)]
    [PropertyTooltip("사운드 재생 기본 볼륨입니다.")]
    public float Volume = 1.0f;

    [Title("Position Rule")]
    [BoxGroup("Position")]
    [PropertyTooltip("플레이어 기준 어느 방향 계열에서 소리를 발생시킬지 결정합니다.")]
    public ESoundJumpScareDirection Direction = ESoundJumpScareDirection.Around360;

    [BoxGroup("Position")]
    [MinValue(0f)]
    [PropertyTooltip("플레이어 기준 최소 재생 거리입니다.")]
    public float MinDistance = 3.0f;

    [BoxGroup("Position")]
    [MinValue(0f)]
    [PropertyTooltip("플레이어 기준 최대 재생 거리입니다.")]
    public float MaxDistance = 6.0f;

    [BoxGroup("Position")]
    [PropertyTooltip("사운드 발생 위치의 높이 오프셋 범위입니다. 랜덤 범위로 사용됩니다.")]
    public Vector2 HeightOffsetRange = new Vector2(-0.2f, 0.6f);

    public bool HasAnyValidSoundEntry()
    {
        if (SoundEntries == null || SoundEntries.Count == 0)
        {
            return false;
        }

        for (int index = 0; index < SoundEntries.Count; index++)
        {
            if (SoundEntries[index].IsValid() == true)
            {
                return true;
            }
        }

        return false;
    }

    private void OnValidate()
    {
        CommonData = EnsureCommon(CommonData);

        CommonData.Type = ESubJumpScareType.Sound;
        CommonData.BlockSameItemAsPrevious = true;

        Volume = Mathf.Clamp01(Volume);

        if (MinDistance < 0f)
        {
            MinDistance = 0f;
        }

        if (MaxDistance < MinDistance)
        {
            MaxDistance = MinDistance;
        }

        if (HeightOffsetRange.x > HeightOffsetRange.y)
        {
            float temp = HeightOffsetRange.x;
            HeightOffsetRange.x = HeightOffsetRange.y;
            HeightOffsetRange.y = temp;
        }
    }
}