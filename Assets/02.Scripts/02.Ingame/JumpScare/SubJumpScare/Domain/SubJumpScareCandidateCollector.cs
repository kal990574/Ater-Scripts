using System.Collections.Generic;

/// <summary>
/// 현재 런타임 상태를 통해 타입별 후보 수집
/// </summary>
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

            if (cooldownState.IsItemCooldownActive(definition.Common.Id) == true)
            {
                continue;
            }

            if (definition.Common.BlockSameItemAsPrevious == true && history.IsSameItem(definition.Common.Id) == true)
            {
                continue;
            }

            if (definition.common.BlockSameTypeAsPrevious == true && history.IsSameType(definition.Common.Type) == true)
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

            if (definition.Clip == null)
            {
                continue;
            }

            if (cooldownState.IsItemCooldownActive(definition.Common.Id) == true)
            {
                continue;
            }

            if (definition.Common.BlockSameIntensityAsPrevious == true
                && history.IsSameIntensity(definition.Common.Intensity) == true)
            {
                continue;
            }

            if (definition.Common.BlockSameItemAsPrevious == true
                && history.IsSameItem(definition.Common.Id) == true)
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

            if (cooldownState.IsItemCooldownActive(definition.Common.Id) == true)
            {
                continue;
            }

            if (definition.Common.BlockSameItemAsPrevious == true
                && history.IsSameItem(definition.Common.Id) == true)
            {
                continue;
            }

            result.Add(definition);
        }

        return result;
    }
}