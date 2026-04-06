using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 서브 점프스케어의 쿨타임 관리
/// </summary>
public class SubJumpScareCooldownState
{
    private float _globalCooldownUntilTime;
    private readonly Dictionary<ESubJumpScareType, float> _typeCooldownUntilMap;
    private readonly Dictionary<string, float> _itemCooldownUntilMap;

    public SubJumpScareCooldownState()
    {
        _globalCooldownUntilTime = 0f;
        _typeCooldownUntilMap = new Dictionary<ESubJumpScareType, float>();
        _itemCooldownUntilMap = new Dictionary<string, float>();

        _typeCooldownUntilMap[ESubJumpScareType.Sound] = 0f;
        _typeCooldownUntilMap[ESubJumpScareType.PostProcess] = 0f;
        _typeCooldownUntilMap[ESubJumpScareType.FakeEnemy] = 0f;
    }

    public bool IsGlobalCooldownActive()
    {
        return Time.time < _globalCooldownUntilTime;
    }

    public float GetRemainingGlobalCooldown()
    {
        return Mathf.Max(0f, _globalCooldownUntilTime - Time.time);
    }

    public bool IsTypeCooldownActive(ESubJumpScareType type)
    {
        if (_typeCooldownUntilMap.ContainsKey(type) == false)
        {
            return false;
        }

        return Time.time < _typeCooldownUntilMap[type];
    }

    public float GetRemainingTypeCooldown(ESubJumpScareType type)
    {
        if (_typeCooldownUntilMap.TryGetValue(type, out float untilTime) == false)
        {
            return 0f;
        }

        return Mathf.Max(0f, untilTime - Time.time);
    }

    public bool IsItemCooldownActive(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId) == true)
        {
            return false;
        }

        if (_itemCooldownUntilMap.ContainsKey(itemId) == false)
        {
            return false;
        }

        return Time.time < _itemCooldownUntilMap[itemId];
    }

    public List<SubJumpScareItemCooldownDebugInfo> GetActiveItemCooldowns()
    {
        List<SubJumpScareItemCooldownDebugInfo> result = new List<SubJumpScareItemCooldownDebugInfo>();

        foreach (KeyValuePair<string, float> pair in _itemCooldownUntilMap)
        {
            float remainingTime = Mathf.Max(0f, pair.Value - Time.time);
            if (remainingTime <= 0f)
            {
                continue;
            }

            result.Add(new SubJumpScareItemCooldownDebugInfo(pair.Key, remainingTime));
        }

        return result;
    }

    public void Commit(SubJumpScareSelectionResult result)
    {
        if (result.IsSuccess == false)
        {
            return;
        }

        if (result.Data == null)
        {
            return;
        }

        float currentTime = Time.time;
        float typeCooldown = result.Data.TypeCooldown;
        float itemCooldown = result.Data.ItemCooldown;
        float globalCooldownContribution = result.Data.GlobalCooldownContribution;

        _typeCooldownUntilMap[result.Data.Type] = currentTime + typeCooldown;
        _itemCooldownUntilMap[result.Data.Id] = currentTime + itemCooldown;
        _globalCooldownUntilTime = currentTime + globalCooldownContribution;
    }

    public void Clear()
    {
        _globalCooldownUntilTime = 0f;

        _typeCooldownUntilMap[ESubJumpScareType.Sound] = 0f;
        _typeCooldownUntilMap[ESubJumpScareType.PostProcess] = 0f;
        _typeCooldownUntilMap[ESubJumpScareType.FakeEnemy] = 0f;

        _itemCooldownUntilMap.Clear();
    }
}
