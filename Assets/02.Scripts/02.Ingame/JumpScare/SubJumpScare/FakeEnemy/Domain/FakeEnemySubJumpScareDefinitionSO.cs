using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "FakeEnemySubJumpScareDefinition", menuName = "Ater/JumpScare/Sub/FakeEnemy Definition")]
[InfoBox("가짜 적 출현 기반 서브 점프스케어 정의입니다. 스폰 거리, 전방 허용 각도, 애니메이션 사용 여부, 포즈 프리팹 목록을 설정합니다.")]
public class FakeEnemySubJumpScareDefinitionSO : ScriptableObject
{
    [Title("Common")]
    [InlineProperty]
    [HideLabel]
    [PropertySpace(8f)]
    public SubJumpScareCommonData Common = new SubJumpScareCommonData();

    [Title("Fake Enemy")]
    [BoxGroup("Spawn")]
    [MinValue(0f)]
    [PropertyTooltip("플레이어로부터 가짜 적이 스폰될 최소 거리입니다.")]
    public float MinSpawnDistance = 3f;

    [BoxGroup("Spawn")]
    [MinValue(0f)]
    [PropertyTooltip("플레이어로부터 가짜 적이 스폰될 최대 거리입니다.")]
    public float MaxSpawnDistance = 8f;

    [BoxGroup("Spawn")]
    [Range(0f, 180f)]
    [PropertyTooltip("플레이어 전방 기준으로 가짜 적이 배치될 수 있는 최대 각도입니다.")]
    public float AllowedForwardAngle = 45f;

    [BoxGroup("Presentation")]
    [PropertyTooltip("가짜 적 등장 시 애니메이션을 사용할지 여부입니다.")]
    public bool UseAnimation = false;

    [BoxGroup("Presentation")]
    [ListDrawerSettings(Expanded = true, DraggableItems = true, ShowIndexLabels = true, NumberOfItemsPerPage = 8)]
    [PropertyTooltip("가짜 적 연출에 사용할 포즈 프리팹 목록입니다. 상황에 따라 이 중 하나를 선택해 사용할 수 있습니다.")]
    public List<GameObject> PosePrefabs = new List<GameObject>();

    private void OnValidate()
    {
        Common.Type = ESubJumpScareType.FakeEnemy;
        Common.BlockSameItemAsPrevious = true;
    }
}