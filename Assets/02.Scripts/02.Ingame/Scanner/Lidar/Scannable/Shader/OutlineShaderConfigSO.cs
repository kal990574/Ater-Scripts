using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "OutlineConfig", menuName = "Ater/Shader/Outline")]
public class OutlineShaderConfigSO : ScriptableObject
{
    public enum OutlineType
    {
        None = 0,
        Simple = 1,
        Constant = 2,
        FadeWithDistance = 3
    }

    [Title("Outline")]
    [ColorUsage(true, true)] public Color AbstractOutlineColor = Color.purple;
    [ColorUsage(true, true)] public Color OnHoverOutlineColor = Color.white;
    public OutlineType Type = OutlineType.Simple;

    [Title("Thickness By Type")]
    [ShowIf("@Type == OutlineType.Simple")]
    [Min(0.0f)] public float SimpleOutlineThickness = 1.0f;
    [ShowIf("@Type == OutlineType.Constant")]
    [Min(0.0f)] public float ConstantOutlineThickness = 1.0f;
    [ShowIf("@Type == OutlineType.FadeWithDistance")]
    [Min(0.0f)] public float FadeWithDistanceOutlineThickness = 1.0f;

    public float GetOutlineThickness()
    {
        return Type switch
        {
            OutlineType.Simple => SimpleOutlineThickness,
            OutlineType.Constant => ConstantOutlineThickness,
            OutlineType.FadeWithDistance => FadeWithDistanceOutlineThickness,
            _ => 0.0f
        };
    }
}
