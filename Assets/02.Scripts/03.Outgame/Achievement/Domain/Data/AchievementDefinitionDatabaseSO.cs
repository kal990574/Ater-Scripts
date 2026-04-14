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
    private readonly Dictionary<string, AchievementDefinition> _definitionMap = new Dictionary<string, AchievementDefinition>();

    public IReadOnlyList<AchievementDefinition> Definitions => _definitions;

    private void OnEnable()
    {
        RebuildCache();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        RebuildCache();
    }
#endif

    public IReadOnlyList<AchievementDefinition> GetAllDefinitions()
    {
        return _definitions;
    }

    public bool TryGetDefinition(string id, out AchievementDefinition definition)
    {
        if (string.IsNullOrWhiteSpace(id) == true)
        {
            definition = null;
            return false;
        }

        if (_definitionMap.Count == 0)
        {
            RebuildCache();
        }

        return _definitionMap.TryGetValue(id, out definition);
    }

    private void RebuildCache()
    {
        _definitionMap.Clear();

        for (int index = 0; index < _definitions.Count; index++)
        {
            AchievementDefinition definition = _definitions[index];
            if (definition == null)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(definition.Id) == true)
            {
                continue;
            }

            if (_definitionMap.ContainsKey(definition.Id) == true)
            {
                continue;
            }

            _definitionMap.Add(definition.Id, definition);
        }
    }
}
