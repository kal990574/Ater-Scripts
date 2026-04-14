using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "SonarOutlineConfig", menuName = "Ater/Shader/SonarOutline")]
public class SonarOutlineConfigSO : ScriptableObject
{
    [Header("Outline")]
    [ColorUsage(true, true)]
    public Color OutlineColor = new(0f, 0.5f, 1f, 1f);
    public float OutlineThickness = 1.5f;
    public float OutlineDuration = 3f;
    public Ease OutlineDecayEase = Ease.InQuad;
}