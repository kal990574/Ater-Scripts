using _02.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

public class SensitivityChannelController: MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private PlayerConfigSO _playerConfig;

    private void Start()
    {
        _slider.value = SensitivitySetting.Sensitivity / _playerConfig.MouseSensitivity;
        _slider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnDestroy()
    {
        _slider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    private void OnSliderChanged(float value)
    {
        SensitivitySetting.Sensitivity = value * _playerConfig.MouseSensitivity;
    }
}
