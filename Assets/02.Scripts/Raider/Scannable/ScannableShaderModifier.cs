using System;
using UnityEngine;

public class ScannableShaderModifier : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField, ColorUsage(true, true)] private Color outlineColor = Color.white;

    private static readonly int _outlineColorID = Shader.PropertyToID("_OutlineColor");
    private static readonly int _textureBlendingCutoffID = Shader.PropertyToID("_BlendingMaskCutOffWhite");

    private MaterialPropertyBlock _mpb;


    private void Awake()
    {
        _mpb = new  MaterialPropertyBlock();
    }

    public void SetOutlineColor()
    {
        _mpb.SetColor(_outlineColorID, outlineColor);
        targetRenderer.SetPropertyBlock(_mpb);
    }

    public void SetOutlineColor(Color outlineColor)
    {
        this.outlineColor = outlineColor;
        SetOutlineColor();
    }

    public void SetBlendCutOff(float cutoff)
    {
        _mpb.SetFloat(_textureBlendingCutoffID, cutoff);
        targetRenderer.SetPropertyBlock(_mpb);
    }
}
