using DG.Tweening;
using System;
using UnityEngine;

[Serializable]
public class InteractTargetShaderConfig
{
    [Header("Blend Cutoff")]
    public string TextureBlendingCutoffName = "_BlendingMaskCutoffWhite";

    [Header("Outline")]
    public string OutlineColorName = "_OutlineColor";
    [ColorUsage(true, true)] public Color AbstractOutlineColor = Color.white;
    [ColorUsage(true, true)] public Color OnHoverOutlineColor = Color.white;
    [ColorUsage(true, true)] public Color OnSonarCheckedOutlineColor = Color.white;
    public string OutlineThicknessName = "_OutlineThickness";
    [Min(0f)] public float OutlineThickPower = 2f;

    [Header("Glitch")]
    public bool ActiveGlitch;
    public string GlitchAmountName = "_GlitchAmount";
    [Min(0f)] public float GlitchAmountPower = 0.1f;

    [Header("Distortion")]
    public bool ActiveDistortion;
    public string DistortionAmountName = "_VertexDistortionAmount";
    [Min(0f)] public float DistortionAmountPower = 0.1f;

    [Header("Hit Blend")]
    public string HitBlendName = "_HitBlend";
    [Min(0f)] public float HitBlendPower = 0.5f;
    public float HitBlendPeak = 1.0f;
    [Min(0f)] public float HitBlendDuration = 0.08f;
    [Min(0f)] public float HitBlendDownDuration = 0.2f;
    public Ease HitBlendUpEase = Ease.OutQuad;
    public Ease HitBlendDownEase = Ease.InQuad;
}
