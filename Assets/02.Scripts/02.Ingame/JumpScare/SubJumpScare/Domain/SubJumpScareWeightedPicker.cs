using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 후보중에서 가중치 기반 랜덤을 통해 선태갛여 반환
/// </summary>
public class SubJumpScareWeightedPicker
{
    public T SelectWeighted<T>(List<T> candidates, Func<T, int> getWeight)
        where T : UnityEngine.Object
    {
        if (candidates == null || candidates.Count == 0)
        {
            return null;
        }

        int totalWeight = 0;

        for (int index = 0; index < candidates.Count; index++)
        {
            int weight = Mathf.Max(0, getWeight(candidates[index]));
            totalWeight += weight;
        }

        if (totalWeight <= 0)
        {
            return null;
        }

        int randomValue = UnityEngine.Random.Range(0, totalWeight);
        int cumulative = 0;

        for (int index = 0; index < candidates.Count; index++)
        {
            int weight = Mathf.Max(0, getWeight(candidates[index]));
            cumulative += weight;

            if (randomValue < cumulative)
            {
                return candidates[index];
            }
        }

        return candidates[candidates.Count - 1];
    }
}