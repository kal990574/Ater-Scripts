using System;
using UnityEngine;

[Serializable]
public class InteractTargetShaderConfig
{
    
    public string TextureBlendingCutoffName = "_BlendingMaskCutoffWhite";
    
    
    public string OutlineColorName = "_OutlineColor";
    [ColorUsage(true, true)] public Color AbstractOutlineColor = Color.white;
    [ColorUsage(true, true)] public Color OnInteractOutlineColor = Color.white;
    [ColorUsage(true, true)] public Color OnSonarCheckedOutlineColor = Color.white;
    
    public string GlitchSpeedName = "_GlitchSpeed";
    public float DefaultGlitchSpeed = 2.0f;
    
    
}