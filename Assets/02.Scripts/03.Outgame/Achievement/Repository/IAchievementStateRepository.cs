using System.Collections.Generic;

public interface IAchievementStateRepository
{
    List<AchievementState> LoadStates(IReadOnlyList<AchievementDefinition> definitions);
    void SaveStates(IReadOnlyList<AchievementState> states);
}
