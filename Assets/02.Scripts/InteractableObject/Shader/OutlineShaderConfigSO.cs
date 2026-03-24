using UnityEngine;

[CreateAssetMenu(fileName = "OutlineConfig", menuName = "Ater/Shader/Outline")]
public class OutlineShaderConfigSO : ScriptableObject
{
    [Header("Outline")]
    [ColorUsage(true, true)] public Color AbstractOutlineColor = Color.purple;
    [ColorUsage(true, true)] public Color OnHoverOutlineColor = Color.white;
}
