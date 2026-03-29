using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class RuntimeStateSetter : MonoBehaviour
{
    private enum StateValueType
    {
        Bool,
        Int,
        String
    }

    [Serializable]
    private class StateEntry
    {
        [SerializeField] private string _key = string.Empty;
        [SerializeField] private StateValueType _valueType = StateValueType.Bool;
        [SerializeField] private bool _boolValue;
        [SerializeField] private int _intValue;
        [SerializeField] private string _stringValue = string.Empty;

        public string Key => _key;
        public StateValueType ValueType => _valueType;
        public bool BoolValue => _boolValue;
        public int IntValue => _intValue;
        public string StringValue => _stringValue;
    }

    [SerializeField] private RuntimeView _runtimeView;
    [SerializeField] private bool _refreshViewAfterApply = true;
    [SerializeField] private List<StateEntry> _entries = new();

    private void Awake()
    {
        if (_runtimeView == null)
        {
            _runtimeView = GetComponentInParent<RuntimeView>();
        }
    }

    public void ApplyAll()
    {
        if (!TryResolveRuntimeView(out RuntimeView runtimeView))
        {
            return;
        }

        foreach (StateEntry entry in _entries)
        {
            ApplyEntry(runtimeView, entry, false);
        }

        if (_refreshViewAfterApply)
        {
            runtimeView.RefreshView();
        }
    }

    public void ApplyAt(int index)
    {
        if (!TryResolveRuntimeView(out RuntimeView runtimeView))
        {
            return;
        }

        if (index < 0 || index >= _entries.Count)
        {
            Debug.LogError($"[{nameof(RuntimeStateSetter)}] Entry index {index} is out of range.", this);
            return;
        }

        ApplyEntry(runtimeView, _entries[index], _refreshViewAfterApply);
    }

    public void ApplyFirst()
    {
        ApplyAt(0);
    }

    private bool TryResolveRuntimeView(out RuntimeView runtimeView)
    {
        runtimeView = _runtimeView;
        if (runtimeView != null)
        {
            return true;
        }

        runtimeView = GetComponent<RuntimeView>();
        _runtimeView = runtimeView;
        if (runtimeView != null)
        {
            return true;
        }

        Debug.LogError($"[{nameof(RuntimeStateSetter)}] {nameof(RuntimeView)} reference is missing.", this);
        return false;
    }

    private void ApplyEntry(RuntimeView runtimeView, StateEntry entry, bool refreshView)
    {
        if (entry == null || string.IsNullOrWhiteSpace(entry.Key))
        {
            return;
        }

        switch (entry.ValueType)
        {
            case StateValueType.Bool:
                runtimeView.SetBoolState(entry.Key, entry.BoolValue, refreshView);
                break;
            case StateValueType.Int:
                runtimeView.SetIntState(entry.Key, entry.IntValue, refreshView);
                break;
            case StateValueType.String:
                runtimeView.SetStringState(entry.Key, entry.StringValue, refreshView);
                break;
        }
    }
}
