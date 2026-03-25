using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItemState
{
    [SerializeField] private List<StateEntry> _entries = new();

    private Dictionary<string, StateEntry> _cachedEntries;

    public InventoryItemState Clone()
    {
        InventoryItemState clone = new InventoryItemState();
        foreach (StateEntry entry in _entries)
        {
            clone._entries.Add(entry.Clone());
        }

        return clone;
    }

    public void ApplyOverrides(InventoryItemState overrides)
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

    public bool GetBool(string key, bool defaultValue = false)
    {
        if (!TryGetEntry(key, out StateEntry entry))
        {
            return defaultValue;
        }

        return entry.BoolValue;
    }

    public int GetInt(string key, int defaultValue = 0)
    {
        if (!TryGetEntry(key, out StateEntry entry))
        {
            return defaultValue;
        }

        return entry.IntValue;
    }

    public string GetString(string key, string defaultValue = "")
    {
        if (!TryGetEntry(key, out StateEntry entry))
        {
            return defaultValue;
        }

        return entry.StringValue;
    }

    public void SetBool(string key, bool value)
    {
        GetOrCreateEntry(key).BoolValue = value;
    }

    public void SetInt(string key, int value)
    {
        GetOrCreateEntry(key).IntValue = value;
    }

    public void SetString(string key, string value)
    {
        GetOrCreateEntry(key).StringValue = value;
    }

    private bool TryGetEntry(string key, out StateEntry entry)
    {
        EnsureCache();
        return _cachedEntries.TryGetValue(key, out entry);
    }

    private StateEntry GetOrCreateEntry(string key)
    {
        EnsureCache();
        if (_cachedEntries.TryGetValue(key, out StateEntry entry))
        {
            return entry;
        }

        entry = new StateEntry { Key = key };
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

        _cachedEntries = new Dictionary<string, StateEntry>(StringComparer.Ordinal);
        foreach (StateEntry entry in _entries)
        {
            if (string.IsNullOrWhiteSpace(entry.Key))
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
            if (string.IsNullOrWhiteSpace(entry.Key))
            {
                continue;
            }

            yield return new StateSnapshot(entry.Key, entry.BoolValue, entry.IntValue, entry.StringValue);
        }
    }

    [Serializable]
    private class StateEntry
    {
        [SerializeField] private string _key;
        [SerializeField] private bool _boolValue;
        [SerializeField] private int _intValue;
        [SerializeField] private string _stringValue;

        public string Key
        {
            get => _key;
            set => _key = value;
        }

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
            return new StateEntry
            {
                _key = _key,
                _boolValue = _boolValue,
                _intValue = _intValue,
                _stringValue = _stringValue
            };
        }
    }

    private readonly struct StateSnapshot
    {
        public StateSnapshot(string key, bool boolValue, int intValue, string stringValue)
        {
            Key = key;
            BoolValue = boolValue;
            IntValue = intValue;
            StringValue = stringValue;
        }

        public string Key { get; }
        public bool BoolValue { get; }
        public int IntValue { get; }
        public string StringValue { get; }
    }
}
