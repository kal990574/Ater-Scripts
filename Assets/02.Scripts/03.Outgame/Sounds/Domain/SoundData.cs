using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class SoundData
{
    [SerializeField]
    private SoundKeyReference _key;
    [SerializeField] private AudioClip _audioClip;
    [SerializeField, Min(0f), Range(0,1)] private float _baseVolume = 1f;
    [SerializeField] private bool _isSpacialClip = true;
    [SerializeField, Min(0f), ShowIf(nameof(_isSpacialClip))] private float _minDistance = 1f;
    [SerializeField, Min(0f) ,ShowIf(nameof(_isSpacialClip))] private float _maxDistance = 15f;

    public string Key => _key;
    public AudioClip AudioClip => _audioClip;
    public float BaseVolume => _baseVolume;
    public bool  IsSpacialClip => _isSpacialClip;
    public float MinDistance => _minDistance;
    public float MaxDistance => _maxDistance;
}
