using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "SonarHighlightConfig", menuName = "Ater/Shader/SonarHighlight")]
public class SonarHighlightConfigSO : ScriptableObject
{
    [Header("Hit Blend")]
    [ColorUsage(true, true)]
    public Color HitColor = new(0f, 0.5f, 1f, 1f);
    public float HitGlow = 2f;
    public float HitBlendPeak = 0.6f;
    public float HitBlendAttackDuration = 1f;
    public Ease HitBlendAttackEase = Ease.OutQuad;
}