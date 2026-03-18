using System;
using UnityEngine;

[DisallowMultipleComponent]
public class AIOShaderModifier : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Renderer targetRenderer;

    [Header("Shader Values")]
    [SerializeField, ColorUsage(true, true)] private Color outlineColor = Color.white;
    [SerializeField, Range(0.0f, 1.0f)] private float blendCutOff = 0.0f;

    private static readonly int OutlineColorID = Shader.PropertyToID("_OutlineColor");
    private static readonly int TextureBlendingCutoffID = Shader.PropertyToID("_BlendingMaskCutoffWhite");

    [SerializeField]private MaterialPropertyBlock materialPropertyBlock;

    private void Awake()
    {
        Init();
    }
    
    
    private void Init()
    {
        if (materialPropertyBlock == null)
        {
            materialPropertyBlock = new MaterialPropertyBlock();
        }

        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }
    }

    
    public void ResetAllProperties()
    {
        targetRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetColor(OutlineColorID, outlineColor);
        materialPropertyBlock.SetFloat(TextureBlendingCutoffID, blendCutOff);
        targetRenderer.SetPropertyBlock(materialPropertyBlock);
    }

    public void SetOutlineColor(Color newOutlineColor)
    {
        outlineColor = newOutlineColor;
        ApplyOutlineColor();
    }

    public void SetBlendCutOff(float newCutOff)
    {
        blendCutOff = newCutOff;
        ApplyBlendCutOff();
        Debug.Log("Blend CutOff: " + blendCutOff);
    }

    public void ResetShaderValues()
    {
        blendCutOff = 0.0f;
        ResetAllProperties();
    }

    private void ApplyOutlineColor()
    {
        targetRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetColor(OutlineColorID, outlineColor);
        targetRenderer.SetPropertyBlock(materialPropertyBlock);
    }

    private void ApplyBlendCutOff()
    {
        targetRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetFloat(TextureBlendingCutoffID, blendCutOff);
        targetRenderer.SetPropertyBlock(materialPropertyBlock);
    }
}