using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class SoundData
{
    private string FoldoutTitle => string.IsNullOrWhiteSpace(Key) ? "Sound Data" : Key;

    [SerializeField]
    [FoldoutGroup("$FoldoutTitle", expanded: false)]
    private SoundKeyReference _key;
    [SerializeField, FoldoutGroup("$FoldoutTitle")] private AudioClip[] _audioClip;
    [SerializeField, Min(0f), Range(0,1), FoldoutGroup("$FoldoutTitle")] private float _baseVolume = 1f;
    [SerializeField, FoldoutGroup("$FoldoutTitle")] private bool _isSpacialClip = true;
    [SerializeField, Min(0f), ShowIf(nameof(_isSpacialClip)), FoldoutGroup("$FoldoutTitle")] private float _minDistance = 1f;
    [SerializeField, Min(0f), ShowIf(nameof(_isSpacialClip)), FoldoutGroup("$FoldoutTitle")] private float _maxDistance = 15f;

    public string Key => _key;
    public AudioClip AudioClip => _audioClip.Length > 0 ? _audioClip[Random.Range(0, _audioClip.Length)] : null;
    public float BaseVolume => _baseVolume;
    public bool  IsSpacialClip => _isSpacialClip;
    public float MinDistance => _minDistance;
    public float MaxDistance => _maxDistance;

    public void SetKey(string key)
    {
        _key.Value = key;
    }

    public bool HasAudioClip()
    {
        return _audioClip != null;
    }
}
