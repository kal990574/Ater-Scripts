using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingPanel : MonoBehaviour
{
    [SerializeField] private AudioChannelController _masterChannel;
    [SerializeField] private AudioChannelController _musicChannel;
    [SerializeField] private AudioChannelController _sfxChannel;

    private void Start()
    {
        _masterChannel.OnVolumeChanged += SoundManager.Instance.SetMasterVolume;
        _musicChannel.OnVolumeChanged += SoundManager.Instance.SetBGMVolume;
        _sfxChannel.OnVolumeChanged += SoundManager.Instance.SetSFXVolume;
    }

    private void OnDestroy()
    {
        _masterChannel.OnVolumeChanged -= SoundManager.Instance.SetMasterVolume;
        _musicChannel.OnVolumeChanged -= SoundManager.Instance.SetBGMVolume;
        _sfxChannel.OnVolumeChanged -= SoundManager.Instance.SetSFXVolume;
    }
}
