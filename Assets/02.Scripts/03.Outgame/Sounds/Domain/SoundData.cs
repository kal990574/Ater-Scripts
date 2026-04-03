using System;
using UnityEngine;

[Serializable]
public class SoundData
{
    [SerializeField] private string _key;
    [SerializeField] private AudioClip _audioClip;

    public string Key => _key;
    public AudioClip AudioClip => _audioClip;
}
