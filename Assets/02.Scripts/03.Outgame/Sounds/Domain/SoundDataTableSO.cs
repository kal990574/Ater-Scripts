using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDataTableSO", menuName = "Ater/Sound/SoundDataTable")]
public class SoundDataTableSO : ScriptableObject
{
    [SerializeField] private List<SoundData> _soundData;

    private Dictionary<string, SoundData> _soundMap;

    private void OnEnable()
    {
        BuildMap();
    }

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
}
