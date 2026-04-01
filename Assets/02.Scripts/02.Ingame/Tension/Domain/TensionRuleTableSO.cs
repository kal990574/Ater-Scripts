using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultTensionRuleTable", menuName = "Ater/JumpScare/Tension Rule Table")]
public class TensionRuleTableSO : ScriptableObject
{
    [Header("Tension Rules")]
    [SerializeField] private List<TensionRule> _tensionRules = new List<TensionRule>();

    [Header("Sync Options")]
    [SerializeField] private bool _autoSyncOnValidate = true;
    [SerializeField] private bool _sortRulesByReason = true;
    [SerializeField] private bool _removeDuplicateReasons = true;

    //자동화용 중복 방지
    private readonly Dictionary<string, TensionRule> _ruleCache = new Dictionary<string, TensionRule>();
    private bool _isCacheBuilt;

    public IReadOnlyList<TensionRule> TensionRules => _tensionRules;

    private void OnEnable()
    {
        RebuildCache();
    }

    [Button]
    private void AutoFill()
    {
        if (_autoSyncOnValidate == true)
        {
            SyncReasonsFromConstants();
        }

        if (_removeDuplicateReasons == true)
        {
            RemoveDuplicateReasons();
        }

        if (_sortRulesByReason == true)
        {
            SortRules();
        }

        RebuildCache();
    }

    public bool TryGetRule(string reason, out TensionRule matchedRule)
    {
        if (string.IsNullOrEmpty(reason) == true)
        {
            matchedRule = null;
            return false;
        }

        if (_isCacheBuilt == false)
        {
            RebuildCache();
        }

        return _ruleCache.TryGetValue(reason, out matchedRule);
    }

    public void RebuildCache()
    {
        _ruleCache.Clear();

        for (int index = 0; index < _tensionRules.Count; index++)
        {
            TensionRule rule = _tensionRules[index];

            if (rule == null)
            {
                continue;
            }

            if (string.IsNullOrEmpty(rule.Reason) == true)
            {
                continue;
            }

            if (_ruleCache.ContainsKey(rule.Reason) == true)
            {
                continue;
            }

            _ruleCache.Add(rule.Reason, rule);
        }

        _isCacheBuilt = true;
    }

    private void SyncReasonsFromConstants()
    {
        List<string> allReasons = GetAllReasonsFromConstants();

        for (int index = 0; index < allReasons.Count; index++)
        {
            string reason = allReasons[index];

            if (ContainsReason(reason) == true)
            {
                continue;
            }

            TensionRule newRule = new TensionRule();
            newRule.SetReason(reason);
            _tensionRules.Add(newRule);
        }
    }

    private bool ContainsReason(string reason)
    {
        for (int index = 0; index < _tensionRules.Count; index++)
        {
            TensionRule rule = _tensionRules[index];

            if (rule == null)
            {
                continue;
            }

            if (rule.Reason == reason)
            {
                return true;
            }
        }

        return false;
    }

    private void RemoveDuplicateReasons()
    {
        HashSet<string> uniqueReasons = new HashSet<string>();
        List<TensionRule> uniqueRules = new List<TensionRule>();

        for (int index = 0; index < _tensionRules.Count; index++)
        {
            TensionRule rule = _tensionRules[index];

            if (rule == null)
            {
                continue;
            }

            if (string.IsNullOrEmpty(rule.Reason) == true)
            {
                uniqueRules.Add(rule);
                continue;
            }

            if (uniqueReasons.Add(rule.Reason) == true)
            {
                uniqueRules.Add(rule);
            }
        }

        _tensionRules = uniqueRules;
    }

    private void SortRules()
    {
        _tensionRules.Sort(CompareRulesByReason);
    }

    private int CompareRulesByReason(TensionRule left, TensionRule right)
    {
        string leftReason = left != null ? left.Reason : string.Empty;
        string rightReason = right != null ? right.Reason : string.Empty;

        return string.Compare(leftReason, rightReason, StringComparison.Ordinal);
    }

    private List<string> GetAllReasonsFromConstants()
    {
        List<string> reasons = new List<string>();

        FieldInfo[] fields = typeof(TensionReasons).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

        for (int index = 0; index < fields.Length; index++)
        {
            FieldInfo field = fields[index];

            if (field.IsLiteral == false)
            {
                continue;
            }

            if (field.IsInitOnly == true)
            {
                continue;
            }

            if (field.FieldType != typeof(string))
            {
                continue;
            }

            string reason = field.GetRawConstantValue() as string;

            if (string.IsNullOrEmpty(reason) == true)
            {
                continue;
            }

            reasons.Add(reason);
        }

        return reasons;
    }
}