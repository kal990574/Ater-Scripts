using System.Collections.Generic;

public class SubJumpScareCandidateCollector
{
    public List<PostProcessSubJumpScareDefinitionSO> GetPostProcessCandidates(
        SubJumpScareDatabaseSO database,
        SubJumpScareContext context,
        SubJumpScareCooldownState cooldownState,
        SubJumpScareHistory history)
    {
        List<PostProcessSubJumpScareDefinitionSO> result = new List<PostProcessSubJumpScareDefinitionSO>();

        if (database == null || context == null || cooldownState == null || history == null)
        {
            return result;
        }

        for (int index = 0; index < database.PostProcessDefinitions.Count; index++)
        {
            PostProcessSubJumpScareDefinitionSO definition = database.PostProcessDefinitions[index];
            if (definition == null)
            {
                continue;
            }

            if (definition.Common.IsValid() == false)
            {
                continue;
            }

            if (definition.Common.IsInTensionRange(context.TotalTension) == false)
            {
                continue;
            }

            if (definition.EffectType == EPostProcessEffectType.None)
            {
                continue;
            }

            if (cooldownState.IsItemCooldownActive(definition.Common.Id))
            {
                continue;
            }

            if (definition.Common.BlockSameItemAsPrevious && history.IsSameItem(definition.Common.Id))
            {
                continue;
            }

            if (definition.Common.BlockSameTypeAsPrevious && history.IsSameType(definition.Common.Type))
            {
                continue;
            }

            result.Add(definition);
        }

        return result;
    }

    public List<SoundSubJumpScareDefinitionSO> GetSoundCandidates(
        SubJumpScareDatabaseSO database,
        SubJumpScareContext context,
        SubJumpScareCooldownState cooldownState,
        SubJumpScareHistory history)
    {
        List<SoundSubJumpScareDefinitionSO> result = new List<SoundSubJumpScareDefinitionSO>();

        if (database == null || context == null || cooldownState == null || history == null)
        {
            return result;
        }

        for (int index = 0; index < database.SoundDefinitions.Count; index++)
        {
            SoundSubJumpScareDefinitionSO definition = database.SoundDefinitions[index];
            if (definition == null)
            {
                continue;
            }

            if (definition.Common.IsValid() == false)
            {
                continue;
            }

            if (definition.Common.IsInTensionRange(context.TotalTension) == false)
            {
                continue;
            }

            if (definition.SoundEntries == null || definition.SoundEntries.Count == 0)
            {
                continue;
            }

            if (cooldownState.IsItemCooldownActive(definition.Common.Id))
            {
                continue;
            }

            if (definition.Common.BlockSameIntensityAsPrevious
                && history.IsSameIntensity(definition.Common.Intensity))
            {
                continue;
            }

            if (definition.Common.BlockSameItemAsPrevious
                && history.IsSameItem(definition.Common.Id))
            {
                continue;
            }

            result.Add(definition);
        }

        return result;
    }

    public List<FakeEnemySubJumpScareDefinitionSO> GetFakeEnemyCandidates(
        SubJumpScareDatabaseSO database,
        SubJumpScareContext context,
        SubJumpScareCooldownState cooldownState,
        SubJumpScareHistory history)
    {
        List<FakeEnemySubJumpScareDefinitionSO> result = new List<FakeEnemySubJumpScareDefinitionSO>();

        if (database == null || context == null || cooldownState == null || history == null)
        {
            return result;
        }

        for (int index = 0; index < database.FakeEnemyDefinitions.Count; index++)
        {
            FakeEnemySubJumpScareDefinitionSO definition = database.FakeEnemyDefinitions[index];
            if (definition == null)
            {
                continue;
            }

            if (definition.Common.IsValid() == false)
            {
                continue;
            }

            if (definition.Common.IsInTensionRange(context.TotalTension) == false)
            {
                continue;
            }

            if (definition.PosePrefabs == null || definition.PosePrefabs.Length == 0)
            {
                continue;
            }

            if (cooldownState.IsItemCooldownActive(definition.Common.Id))
            {
                continue;
            }

            if (definition.Common.BlockSameItemAsPrevious
                && history.IsSameItem(definition.Common.Id))
            {
                continue;
            }

            result.Add(definition);
        }

        return result;
    }
}
