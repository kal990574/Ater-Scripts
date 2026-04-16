using System;
using UnityEngine;

public static class SensitivitySetting
{
    private const string SensitivityKey = "Sensitivity";
    private static float _defaultSensitivity;

    public static event Action<float> OnSensitivityChanged;

    public static void Init(float defaultSensitivity)
    {
        _defaultSensitivity = defaultSensitivity;
    }

    public static float Sensitivity
    {
        get => PlayerPrefs.GetFloat(SensitivityKey, _defaultSensitivity);
        set
        {
            PlayerPrefs.SetFloat(SensitivityKey, value);
            OnSensitivityChanged?.Invoke(value);
        }
    }
}