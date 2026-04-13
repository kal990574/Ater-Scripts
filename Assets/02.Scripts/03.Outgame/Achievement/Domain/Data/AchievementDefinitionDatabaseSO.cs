using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementDefinitionDatabase", menuName = "Ater/Achievement/Definition Database")]
public class AchievementDefinitionDatabaseSO : ScriptableObject
{
    [SerializeField] private List<AchievementDefinition> _definitions = new List<AchievementDefinition>();

    public IReadOnlyList<AchievementDefinition> Definitions => _definitions;
}
