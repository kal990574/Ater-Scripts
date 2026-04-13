using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(
    fileName = "AchievementDefinitionDatabase",
    menuName = "Ater/Achievement/Achievement Definition Database")]
public class AchievementDefinitionDatabaseSO : ScriptableObject
{
    [Title("Achievement Definitions")]
    [SerializeField] private List<AchievementDefinition> _definitions = new List<AchievementDefinition>();

    public IReadOnlyList<AchievementDefinition> Definitions => _definitions;
}