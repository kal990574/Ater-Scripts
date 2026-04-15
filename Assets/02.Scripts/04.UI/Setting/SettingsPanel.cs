using Michsky.UI.Dark;
using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
    [Header("Audio Sliders")]
    [SerializeField] private SliderManager _masterVolumeSlider;
    [SerializeField] private SliderManager _musicVolumeSlider;
    [SerializeField] private SliderManager _sfxVolumeSlider;

    private void Awake()
    {
        _masterVolumeSlider.mainSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        _musicVolumeSlider.mainSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        _sfxVolumeSlider.mainSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    private void Start()
    {
        OnMasterVolumeChanged(_masterVolumeSlider.mainSlider.value);
        OnMusicVolumeChanged(_musicVolumeSlider.mainSlider.value);
        OnSFXVolumeChanged(_sfxVolumeSlider.mainSlider.value);
    }

    private void OnDestroy()
    {
        _masterVolumeSlider.mainSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        _musicVolumeSlider.mainSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        _sfxVolumeSlider.mainSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
    }

    private void OnMasterVolumeChanged(float value)
    {
        SoundManager.Instance.SetMasterVolume(value);
    }

    private void OnMusicVolumeChanged(float value)
    {
        SoundManager.Instance.SetBGMVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        SoundManager.Instance.SetSFXVolume(value);
    }
}
