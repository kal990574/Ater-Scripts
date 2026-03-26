using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemState
{
    [SerializeField] private List<StateEntry> _entries = new();

    private Dictionary<StateKeySO, StateEntry> _cachedEntries;

    public ItemState Clone()
    {
        ItemState clone = new ItemState();
        foreach (StateEntry entry in _entries)
        {
            clone._entries.Add(entry.Clone());
        }

        return clone;
    }

    public void ApplyOverrides(ItemState overrides)
    {
        if (overrides == null)
        {
            return;
        }

        foreach (StateSnapshot snapshot in overrides.GetSnapshots())
        {
            StateEntry entry = GetOrCreateEntry(snapshot.Key);
            entry.BoolValue = snapshot.BoolValue;
            entry.IntValue = snapshot.IntValue;
            entry.StringValue = snapshot.StringValue;
        }
    }

    public bool GetBool(StateKeySO key, bool defaultValue = false)
    {
        if (!TryGetEntry(key, out StateEntry entry))
        {
            return defaultValue;
        }

        return entry.BoolValue;
    }

    public int GetInt(StateKeySO key, int defaultValue = 0)
    {
        if (!TryGetEntry(key, out StateEntry entry))
        {
            return defaultValue;
        }

        return entry.IntValue;
    }

    public string GetString(StateKeySO key, string defaultValue = "")
    {
        if (!TryGetEntry(key, out StateEntry entry))
        {
            return defaultValue;
        }

        return entry.StringValue;
    }

    public bool HasKey(StateKeySO key)
    {
        return TryGetEntry(key, out _);
    }

    public void SetBool(StateKeySO key, bool value)
    {
        GetOrCreateEntry(key).BoolValue = value;
    }

    public void SetInt(StateKeySO key, int value)
    {
        GetOrCreateEntry(key).IntValue = value;
    }

    public void SetString(StateKeySO key, string value)
    {
        GetOrCreateEntry(key).StringValue = value;
    }

    private bool TryGetEntry(StateKeySO key, out StateEntry entry)
    {
        if (key == null)
        {
            entry = null;
            return false;
        }

        EnsureCache();
        return _cachedEntries.TryGetValue(key, out entry);
    }

    private StateEntry GetOrCreateEntry(StateKeySO key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        EnsureCache();
        if (_cachedEntries.TryGetValue(key, out StateEntry entry))
        {
            return entry;
        }

        entry = new StateEntry(key);
        _entries.Add(entry);
        _cachedEntries[key] = entry;
        return entry;
    }

    private void EnsureCache()
    {
        if (_cachedEntries != null)
        {
            return;
        }

        _cachedEntries = new Dictionary<StateKeySO, StateEntry>();
        foreach (StateEntry entry in _entries)
        {
            if (entry.Key == null)
            {
                continue;
            }

            _cachedEntries[entry.Key] = entry;
        }
    }

    private IEnumerable<StateSnapshot> GetSnapshots()
    {
        foreach (StateEntry entry in _entries)
        {
            if (entry.Key == null)
            {
                continue;
            }

            yield return new StateSnapshot(entry.Key, entry.BoolValue, entry.IntValue, entry.StringValue);
        }
    }

    [Serializable]
    private class StateEntry
    {
        [SerializeField] private StateKeySO _key;
        [SerializeField] private bool _boolValue;
        [SerializeField] private int _intValue;
        [SerializeField] private string _stringValue;

        public StateEntry(StateKeySO key)
        {
            _key = key;
        }

        public StateKeySO Key => _key;

        public bool BoolValue
        {
            get => _boolValue;
            set => _boolValue = value;
        }

        public int IntValue
        {
            get => _intValue;
            set => _intValue = value;
        }

        public string StringValue
        {
            get => _stringValue;
            set => _stringValue = value;
        }

        public StateEntry Clone()
        {
            return new StateEntry(_key)
            {
                _boolValue = _boolValue,
                _intValue = _intValue,
                _stringValue = _stringValue
            };
        }
    }

    private readonly struct StateSnapshot
    {
        public StateSnapshot(StateKeySO key, bool boolValue, int intValue, string stringValue)
        {
            Key = key;
            BoolValue = boolValue;
            IntValue = intValue;
            StringValue = stringValue;
        }

        public StateKeySO Key { get; }
        public bool BoolValue { get; }
        public int IntValue { get; }
        public string StringValue { get; }
    }
}
