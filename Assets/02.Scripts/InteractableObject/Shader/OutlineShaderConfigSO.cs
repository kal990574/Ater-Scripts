using UnityEngine;

[CreateAssetMenu(fileName = "OutlineConfig", menuName = "Ater/Shader/Outline")]
public class OutlineShaderConfigSO : ScriptableObject
{
    [Header("Outline")]
    [ColorUsage(true, true)] public Color AbstractOutlineColor = Color.white;
    [ColorUsage(true, true)] public Color OnHoverOutlineColor = Color.white;
    [ColorUsage(true, true)] public Color OnSonarCheckedOutlineColor = Color.white;
    
    [Min(0f)] public float OutlineThickPower = 2f;

}
