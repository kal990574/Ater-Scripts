using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 서브 점프스케어의 쿨타임 관리
/// </summary>
using System.Collections.Generic;
using UnityEngine;

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

    public bool IsTypeCooldownActive(ESubJumpScareType type)
    {
        if (_typeCooldownUntilMap.ContainsKey(type) == false)
        {
            return false;
        }

        return Time.time < _typeCooldownUntilMap[type];
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
