using DG.Tweening;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ShaderConfig", menuName = "Ater/Shader/Scan")]
public class ScanShaderConfigSO : ScriptableObject
{
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
