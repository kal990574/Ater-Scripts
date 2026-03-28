using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemState
{
    [SerializeField] private List<StateEntry> _entries = new();

    private Dictionary<string, StateEntry> _cachedEntries;

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
            StateEntry entry = GetOrCreateEntry(snapshot.KeyId);
            entry.BoolValue = snapshot.BoolValue;
            entry.IntValue = snapshot.IntValue;
            entry.StringValue = snapshot.StringValue;
        }
    }

    public bool GetBool(StateKeySO key, bool defaultValue = false)
    {
        if (!TryGetEntry(GetKeyId(key), out StateEntry entry))
        {
            return defaultValue;
        }

        return entry.BoolValue;
    }

    public int GetInt(StateKeySO key, int defaultValue = 0)
    {
        if (!TryGetEntry(GetKeyId(key), out StateEntry entry))
        {
            return defaultValue;
        }

        return entry.IntValue;
    }

    public string GetString(StateKeySO key, string defaultValue = "")
    {
        if (!TryGetEntry(GetKeyId(key), out StateEntry entry))
        {
            return defaultValue;
        }

        return entry.StringValue;
    }

    public bool HasKey(StateKeySO key)
    {
        return TryGetEntry(GetKeyId(key), out _);
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

    public List<ItemStateValueSaveData> CaptureSaveData()
    {
        List<ItemStateValueSaveData> saveData = new List<ItemStateValueSaveData>();
        foreach (StateEntry entry in _entries)
        {
            string keyId = entry.KeyId;
            if (string.IsNullOrEmpty(keyId))
            {
                continue;
            }

            saveData.Add(new ItemStateValueSaveData
            {
                Key = keyId,
                BoolValue = entry.BoolValue,
                IntValue = entry.IntValue,
                StringValue = entry.StringValue
            });
        }

        return saveData;
    }

    public void RestoreSaveData(List<ItemStateValueSaveData> saveData)
    {
        _entries.Clear();
        _cachedEntries = null;

        if (saveData == null)
        {
            return;
        }

        foreach (ItemStateValueSaveData entry in saveData)
        {
            if (entry == null || string.IsNullOrEmpty(entry.Key))
            {
                continue;
            }

            _entries.Add(new StateEntry(entry.Key)
            {
                BoolValue = entry.BoolValue,
                IntValue = entry.IntValue,
                StringValue = entry.StringValue
            });
        }
    }

    private bool TryGetEntry(string keyId, out StateEntry entry)
    {
        if (string.IsNullOrEmpty(keyId))
        {
            entry = null;
            return false;
        }

        EnsureCache();
        return _cachedEntries.TryGetValue(keyId, out entry);
    }

    private StateEntry GetOrCreateEntry(StateKeySO key)
    {
        string keyId = GetKeyId(key);
        if (string.IsNullOrEmpty(keyId))
        {
            throw new ArgumentNullException(nameof(key));
        }

        EnsureCache();
        if (_cachedEntries.TryGetValue(keyId, out StateEntry entry))
        {
            return entry;
        }

        entry = new StateEntry(key);
        _entries.Add(entry);
        _cachedEntries[keyId] = entry;
        return entry;
    }

    private StateEntry GetOrCreateEntry(string keyId)
    {
        if (string.IsNullOrEmpty(keyId))
        {
            throw new ArgumentNullException(nameof(keyId));
        }

        EnsureCache();
        if (_cachedEntries.TryGetValue(keyId, out StateEntry entry))
        {
            return entry;
        }

        entry = new StateEntry(keyId);
        _entries.Add(entry);
        _cachedEntries[keyId] = entry;
        return entry;
    }

    private void EnsureCache()
    {
        if (_cachedEntries != null)
        {
            return;
        }

        _cachedEntries = new Dictionary<string, StateEntry>();
        foreach (StateEntry entry in _entries)
        {
            string keyId = entry.KeyId;
            if (string.IsNullOrEmpty(keyId))
            {
                continue;
            }

            _cachedEntries[keyId] = entry;
        }
    }

    private IEnumerable<StateSnapshot> GetSnapshots()
    {
        foreach (StateEntry entry in _entries)
        {
            string keyId = entry.KeyId;
            if (string.IsNullOrEmpty(keyId))
            {
                continue;
            }

            yield return new StateSnapshot(keyId, entry.BoolValue, entry.IntValue, entry.StringValue);
        }
    }

    private static string GetKeyId(StateKeySO key)
    {
        return key != null ? key.PersistentKey : null;
    }

    [Serializable]
    private class StateEntry
    {
        [SerializeField] private StateKeySO _key;
        [SerializeField] private string _keyId;
        [SerializeField] private bool _boolValue;
        [SerializeField] private int _intValue;
        [SerializeField] private string _stringValue;

        public StateEntry(StateKeySO key)
        {
            _key = key;
            _keyId = GetKeyId(key);
        }

        public StateEntry(string keyId)
        {
            _keyId = keyId;
        }

        public string KeyId => string.IsNullOrEmpty(_keyId) ? GetKeyId(_key) : _keyId;

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
            return new StateEntry(KeyId)
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
        public StateSnapshot(string keyId, bool boolValue, int intValue, string stringValue)
        {
            KeyId = keyId;
            BoolValue = boolValue;
            IntValue = intValue;
            StringValue = stringValue;
        }

        public string KeyId { get; }
        public bool BoolValue { get; }
        public int IntValue { get; }
        public string StringValue { get; }
    }
}
