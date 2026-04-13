using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "SonarHighlightConfig", menuName = "Ater/Shader/SonarHighlight")]
public class SonarHighlightConfigSO : ScriptableObject
{
    [Header("Timing")]
    [Tooltip("소나 파동 도달 후 효과 유지 시간")]
    public float HighlightDuration = 7f;

    [Tooltip("효과 페이드아웃 시간")]
    public float FadeOutDuration = 1.5f;

    [Header("Outline")]
    [ColorUsage(true, true)]
    public Color SonarOutlineColor = new(0f, 0.5f, 1f, 1f);
    public float OutlineThickness = 1.5f;

    [Header("Hit Blend")]
    public float HitBlendPeak = 0.6f;
    public float HitBlendAttackDuration = 0.1f;
    public Ease HitBlendAttackEase = Ease.OutQuad;
}