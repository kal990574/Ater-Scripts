using System;
using System.Collections.Generic;

public class AchievementDefinitionRepository : IAchievementDefinitionRepository
{
    private readonly AchievementDefinitionDatabaseSO _database;
    private readonly Dictionary<AchievementId, AchievementDefinition> _cachedDefinitions;

    public AchievementDefinitionRepository(AchievementDefinitionDatabaseSO database)
    {
        _database = database;
        _cachedDefinitions = new Dictionary<AchievementId, AchievementDefinition>();

        if (_database == null)
        {
            return;
        }

        IReadOnlyList<AchievementDefinition> definitions = _database.Definitions;
        for (int index = 0; index < definitions.Count; index++)
        {
            AchievementDefinition definition = definitions[index];

            if (definition == null)
            {
                continue;
            }

            if (_cachedDefinitions.ContainsKey(definition.Id) == true)
            {
                continue;
            }

            _cachedDefinitions.Add(definition.Id, definition);
        }
    }

    public IReadOnlyList<AchievementDefinition> GetAllDefinitions()
    {
        if (_database == null)
        {
            return Array.Empty<AchievementDefinition>();
        }

        return _database.Definitions;
    }

    public bool TryGetDefinition(AchievementId id, out AchievementDefinition definition)
    {
        return _cachedDefinitions.TryGetValue(id, out definition);
    }
}
