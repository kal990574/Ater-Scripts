using System.Collections.Generic;

public interface IAchievementDefinitionRepository
{
    IReadOnlyList<AchievementDefinition> GetAllDefinitions();
    bool TryGetDefinition(AchievementId id, out AchievementDefinition definition);
}
