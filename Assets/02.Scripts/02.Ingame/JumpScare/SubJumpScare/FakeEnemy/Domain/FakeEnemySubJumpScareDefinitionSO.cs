using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "FakeEnemySubJumpScareDefinition", menuName = "Ater/JumpScare/Sub/FakeEnemy Definition")]
[InfoBox("가짜 적 기반 서브 점프스케어 정의입니다.")]
public class FakeEnemySubJumpScareDefinitionSO : SubJumpScareDefinitionSOBase
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

    [Title("Spawn Settings")]
    [BoxGroup("Distance")]
    [MinValue(0f)]
    public float MinSpawnDistance = 3f;

    [BoxGroup("Distance")]
    [MinValue(0f)]
    public float MaxSpawnDistance = 8f;

    [BoxGroup("Angle")]
    [Range(0f, 180f)]
    public float AllowedForwardAngle = 60f;

    [Title("Presentation")]
    [AssetsOnly]
    public GameObject[] PosePrefabs;

    private void OnValidate()
    {
        common = EnsureCommon(common);

        common.Type = ESubJumpScareType.FakeEnemy;
        common.BlockSameItemAsPrevious = true;

        MinSpawnDistance = Mathf.Max(0.0f, MinSpawnDistance);
        MaxSpawnDistance = Mathf.Max(MinSpawnDistance, MaxSpawnDistance);
    }
}