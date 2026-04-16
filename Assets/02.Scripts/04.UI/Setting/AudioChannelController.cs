using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioChannelController : MonoBehaviour
{
    private const float MinVolume = 0.0001f;

    [SerializeField] private Slider _slider;
    [SerializeField] private Toggle _muteToggle;
    [SerializeField] private string _mutePrefKey;

    public event Action<float> OnVolumeChanged;

    private void Awake()
    {
        _slider.onValueChanged.AddListener(OnSliderChanged);
        _muteToggle.onValueChanged.AddListener(OnMuteChanged);
    }

    private void Start()
    {
        _muteToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt(_mutePrefKey, 1) == 1);
        ApplyVolume();
    }

    private void OnDestroy()
    {
        _slider.onValueChanged.RemoveListener(OnSliderChanged);
        _muteToggle.onValueChanged.RemoveListener(OnMuteChanged);
    }

    private void OnSliderChanged(float value) => ApplyVolume();

    private void OnMuteChanged(bool isOn)
    {
        PlayerPrefs.SetInt(_mutePrefKey, isOn ? 1 : 0);
        ApplyVolume();
    }

    private void ApplyVolume()
    {
        float volume = _muteToggle.isOn ? _slider.value : MinVolume;
        OnVolumeChanged?.Invoke(volume);
    }
}
