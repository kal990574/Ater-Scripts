using DG.Tweening;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ShaderConfig", menuName = "Ater/Interact/ShaderConfig")]
public class InteractTargetShaderConfig : ScriptableObject
{
    [Header("Outline")]
    [ColorUsage(true, true)] public Color AbstractOutlineColor = Color.white;
    [ColorUsage(true, true)] public Color OnHoverOutlineColor = Color.white;
    [ColorUsage(true, true)] public Color OnSonarCheckedOutlineColor = Color.white;
    [Min(0f)] public float OutlineThickPower = 2f;
    
    [Header("Glitch")]
    public bool ActiveGlitch;
    [Min(0f)] public float GlitchAmountPower = 0.1f;

    [Header("Distortion")]
    public bool ActiveDistortion;
    [Min(0f)] public float DistortionAmountPower = 0.1f;

    [Header("Hit Blend")]
    [Min(0f)] public float HitBlendPower = 0.5f;
    public float HitBlendPeak = 1.0f;
    [Min(0f)] public float HitBlendDuration = 0.08f;
    [Min(0f)] public float HitBlendDownDuration = 0.2f;
    public Ease HitBlendUpEase = Ease.OutQuad;
    public Ease HitBlendDownEase = Ease.InQuad;
}
