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

    public bool TryGetFakeEnemyDefinition(string id, out FakeEnemySubJumpScareDefinitionSO definition)
    {
        for (int index = 0; index < FakeEnemyDefinitions.Count; index++)
        {
            FakeEnemySubJumpScareDefinitionSO current = FakeEnemyDefinitions[index];

            if (current == null)
            {
                continue;
            }

            if (current.Common == null)
            {
                continue;
            }

            if (string.Equals(current.Common.Id, id, System.StringComparison.Ordinal) == true)
            {
                definition = current;
                return true;
            }
        }

        definition = null;
        return false;
    }
}