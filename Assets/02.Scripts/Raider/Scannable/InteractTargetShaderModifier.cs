using System;
using UnityEngine;
public class InteractTargetShaderModifier : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AllInOneShaderController shaderPropertyController;

    [Header("Shader Values")]
    [SerializeField] private InteractTargetShaderConfig config;

    private void Awake()
    {
        if (shaderPropertyController == null)
        {
            shaderPropertyController = GetComponentInChildren<AllInOneShaderController>();
        }
    }

    //추후 변경(추상화 상태일경우 ,호버상태일경우, 소나스캔된 경우)
    public void SetOutlineColor(Color newOutlineColor)
    {
        shaderPropertyController.SetColor(config.OutlineColorName, newOutlineColor);
    }

    public void SetBlendCutOff(float newCutOff)
    {
        shaderPropertyController.SetFloat(config.TextureBlendingCutoffName, newCutOff);
    }

    public void SetGlitchSpeedByRatio(float ratio)
    {
        float glitchSpeed = config.DefaultGlitchSpeed * ratio;
        shaderPropertyController.SetFloat(config.GlitchSpeedName, glitchSpeed);
    }
}