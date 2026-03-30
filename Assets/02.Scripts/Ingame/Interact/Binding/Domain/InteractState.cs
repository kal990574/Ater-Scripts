using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public class InteractState
{
    [SerializeField] private List<StateEntry> _entries = new();

    private Dictionary<string, StateEntry> _cachedEntries;

    public InteractState Clone()
    {
        InteractState clone = new InteractState();
        foreach (StateEntry entry in _entries)
        {
            clone._entries.Add(entry.Clone());
        }

        return clone;
    }

    public void ApplyOverrides(InteractState overrides)
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

    public bool HasKey(string key)
    {
        return TryGetEntry(key, out _);
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

    public string ToDebugString()
    {
        if (_entries == null || _entries.Count == 0)
        {
            return "[]";
        }

        StringBuilder builder = new StringBuilder();
        builder.Append('[');

        bool isFirst = true;
        foreach (StateEntry entry in _entries)
        {
            if (string.IsNullOrWhiteSpace(entry.Key))
            {
                continue;
            }

            if (!isFirst)
            {
                builder.Append(", ");
            }

            builder.Append(entry.Key);
            builder.Append("={bool:");
            builder.Append(entry.BoolValue);
            builder.Append(", int:");
            builder.Append(entry.IntValue);
            builder.Append(", string:\"");
            builder.Append(entry.StringValue);
            builder.Append("\"}");
            isFirst = false;
        }

        builder.Append(']');
        return builder.ToString();
    }

    private bool TryGetEntry(string key, out StateEntry entry)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            entry = null;
            return false;
        }

        EnsureCache();
        return _cachedEntries.TryGetValue(key, out entry);
    }

    private StateEntry GetOrCreateEntry(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("State key cannot be null or whitespace.", nameof(key));
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

        public StateEntry(string key)
        {
            _key = key;
        }

        public string Key => _key;

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
