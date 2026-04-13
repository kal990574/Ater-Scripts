using System.Collections.Generic;

public interface IAchievementDefinitionRepository
{
    IReadOnlyList<AchievementDefinition> GetAllDefinitions();
    bool TryGetDefinition(string id, out AchievementDefinition definition);
}
