using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class InteractTargetShaderConfig
{
    
    public string TextureBlendingCutoffName = "_BlendingMaskCutoffWhite";
    
    
    public string OutlineColorName = "_OutlineColor";
    [ColorUsage(true, true)] public Color AbstractOutlineColor = Color.white;
    [ColorUsage(true, true)] public Color OnHoverOutlineColor = Color.white;
    [ColorUsage(true, true)] public Color OnSonarCheckedOutlineColor = Color.white;
    
    public string OutlineThicknessName = "_OutlineThickness";
     public float OutlineThickPower = 2f;
    
    public string GlitchAmountName = "_GlitchAmount";
    public float GlitchAmountPower = 0.1f;
    
    public string HitBlendName = "_HitBlend";
    public float HitBlendPower = 0.5f;


}