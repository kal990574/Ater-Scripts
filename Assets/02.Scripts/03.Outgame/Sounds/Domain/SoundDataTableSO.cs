using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDataTableSO", menuName = "Ater/Sound/SoundDataTable")]
public class SoundDataTableSO : ScriptableObject
{
    [SerializeField]
    [ListDrawerSettings(Expanded = true)]
    private List<SoundData> _soundData = new();

    private Dictionary<string, SoundData> _soundMap;

    private void OnEnable()
    {
        BuildMap();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        EnsureAllSoundKeysExist();
        SortBySoundKeyOrder();
        LogEntriesWithoutAudioClip();
        BuildMap();
    }
#endif

    public AudioClip GetClip(string key)
    {
        SoundData soundData = GetSoundData(key);
        return soundData != null ? soundData.AudioClip : null;
    }

    public SoundData GetSoundData(string key)
    {
        if (_soundMap == null)
        {
            BuildMap();
        }

        if (_soundMap.TryGetValue(key, out SoundData soundData))
        {
            return soundData;
        }

        Debug.LogWarning($"[SoundDatabase] Unregistered SoundKey: {key}");
        return null;
    }

    private void BuildMap()
    {
        _soundMap = new Dictionary<string, SoundData>();

        foreach (SoundData data in _soundData)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.Key))
            {
                continue;
            }

            if (_soundMap.ContainsKey(data.Key))
            {
                Debug.LogWarning($"[SoundDatabase] Duplicated SoundKey: {data.Key}");
                continue;
            }

            _soundMap[data.Key] = data;
        }
    }

#if UNITY_EDITOR
    private void EnsureAllSoundKeysExist()
    {
        if (_soundData == null)
        {
            _soundData = new List<SoundData>();
        }

        HashSet<string> existingKeys = new HashSet<string>();
        for (int index = 0; index < _soundData.Count; index++)
        {
            SoundData data = _soundData[index];
            if (data == null || string.IsNullOrWhiteSpace(data.Key))
            {
                continue;
            }

            existingKeys.Add(data.Key);
        }

        bool hasAdded = false;
        foreach (string soundKey in SoundKeyDropdown.GetValues().Distinct())
        {
            if (string.IsNullOrWhiteSpace(soundKey) || existingKeys.Contains(soundKey))
            {
                continue;
            }

            SoundData newData = new SoundData();
            newData.SetKey(soundKey);
            _soundData.Add(newData);
            hasAdded = true;
        }

        if (hasAdded == true)
        {
            Debug.Log($"[SoundDatabase] SoundKey 목록 기준으로 누락된 항목을 자동 추가했습니다.", this);
        }
    }

    private void LogEntriesWithoutAudioClip()
    {
        if (_soundData == null)
        {
            return;
        }

        for (int index = 0; index < _soundData.Count; index++)
        {
            SoundData data = _soundData[index];
            if (data == null || string.IsNullOrWhiteSpace(data.Key))
            {
                continue;
            }

            if (data.HasAudioClip() == false)
            {
                Debug.LogWarning($"[SoundDatabase] AudioClip 이 비어 있는 SoundData: {data.Key}", this);
            }
        }
    }

    private void SortBySoundKeyOrder()
    {
        if (_soundData == null || _soundData.Count <= 1)
        {
            return;
        }

        Dictionary<string, int> keyOrderMap = SoundKeyDropdown
            .GetValues()
            .Distinct()
            .Select((key, index) => new { key, index })
            .ToDictionary(item => item.key, item => item.index);

        _soundData = _soundData
            .OrderBy(data =>
            {
                if (data == null || string.IsNullOrWhiteSpace(data.Key))
                {
                    return int.MaxValue;
                }

                if (keyOrderMap.TryGetValue(data.Key, out int order) == true)
                {
                    return order;
                }

                return int.MaxValue - 1;
            })
            .ThenBy(data => data?.Key)
            .ToList();
    }
#endif
}
