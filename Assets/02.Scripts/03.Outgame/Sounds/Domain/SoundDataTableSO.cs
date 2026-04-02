using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDataTableSO", menuName = "Ater/Sound/SoundDataTable")]
public class SoundDataTableSO : ScriptableObject
{
    [SerializeField] private List<SoundData> _soundData;

    private Dictionary<string, AudioClip> _soundMap;

    private void OnEnable()
    {
        BuildMap();
    }

    public AudioClip GetClip(string key)
    {
        if (_soundMap == null)
        {
            BuildMap();
        }

        if (_soundMap.TryGetValue(key, out AudioClip clip))
        {
            return clip;
        }

        Debug.LogWarning($"[SoundDatabase] 등록되지 않은 SoundKey: {key}");
        return null;
    }
    private void BuildMap()
    {
        _soundMap = new Dictionary<string, AudioClip>();

        foreach (SoundData data in _soundData)
        {
            if (_soundMap.ContainsKey(data.Key))
            {
                Debug.LogWarning($"[SoundDatabase] 중복된 SoundKey: {data.Key}");
                continue;
            }
            _soundMap[data.Key] = data.AudioClip;
        }
    }
}
