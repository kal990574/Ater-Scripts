using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingPanel : MonoBehaviour
{
    [SerializeField] private AudioChannelController _masterChannel;
    [SerializeField] private AudioChannelController _musicChannel;
    [SerializeField] private AudioChannelController _sfxChannel;
    [SerializeField] private AudioChannelController _hintChannel;

    private void Start()
    {
        _masterChannel.OnVolumeChanged += SoundManager.Instance.SetMasterVolume;
        _musicChannel.OnVolumeChanged += SoundManager.Instance.SetBGMVolume;
        _sfxChannel.OnVolumeChanged += SoundManager.Instance.SetSFXVolume;
        _hintChannel.OnVolumeChanged += SoundManager.Instance.SetHintVolume;
    }

    private void OnDestroy()
    {
        if (SoundManager.Instance == null) return;

        _masterChannel.OnVolumeChanged -= SoundManager.Instance.SetMasterVolume;
        _musicChannel.OnVolumeChanged -= SoundManager.Instance.SetBGMVolume;
        _sfxChannel.OnVolumeChanged -= SoundManager.Instance.SetSFXVolume;
        _hintChannel.OnVolumeChanged -= SoundManager.Instance.SetHintVolume;
    }
}
