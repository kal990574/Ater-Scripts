using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class InteractTargetShaderConfig
{
    public string TextureBlendingCutoffName = "_BlendingMaskCutoffWhite";
    [Space]
    public string OutlineColorName = "_OutlineColor";
    [ColorUsage(true, true)] public Color AbstractOutlineColor = Color.white;
    [ColorUsage(true, true)] public Color OnHoverOutlineColor = Color.white;
    [ColorUsage(true, true)] public Color OnSonarCheckedOutlineColor = Color.white;
    [Space]
    public string OutlineThicknessName = "_OutlineThickness";
    public float OutlineThickPower = 2f;
    [Space]
     public bool ActiveGlitch = false;
    public string GlitchAmountName = "_GlitchAmount";
    public float GlitchAmountPower = 0.1f;
    [Space]
    public bool ActiveDistortion = false;
    public string DistortionAmountName = "_VertexDistortionAmount";
    public float DistortionAmountPower = 0.1f;
    [Space]
    public string HitBlendName = "_HitBlend";
    public float HitBlendPower = 0.5f;
    [Space]
    public float HitBlendPeak = 1.0f;
    public float HitBlendDuration = 0.08f;
    public float HitBlendDownDuration = 0.2f;
    public Ease HitBlendUpEase = Ease.OutQuad;
    public Ease HitBlendDownEase = Ease.InQuad;
}