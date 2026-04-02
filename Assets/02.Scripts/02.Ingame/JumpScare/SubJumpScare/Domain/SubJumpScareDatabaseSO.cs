using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SubJumpScareDatabase", menuName = "Ater/JumpScare/Sub/Database")]
public class SubJumpScareDatabaseSO : ScriptableObject
{
    [Header("Sound Definitions")]
    public List<SoundSubJumpScareDefinitionSO> SoundDefinitions = new List<SoundSubJumpScareDefinitionSO>();

    [Header("Post Process Definitions")]
    public List<PostProcessSubJumpScareDefinitionSO> PostProcessDefinitions = new List<PostProcessSubJumpScareDefinitionSO>();

    [Header("Fake Enemy Definitions")]
    public List<FakeEnemySubJumpScareDefinitionSO> FakeEnemyDefinitions = new List<FakeEnemySubJumpScareDefinitionSO>();
}