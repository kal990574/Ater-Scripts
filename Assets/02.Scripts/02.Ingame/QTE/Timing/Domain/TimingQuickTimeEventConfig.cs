using UnityEngine;

[CreateAssetMenu(fileName = "TimingConfig", menuName = "QTE/Config/Timing")]
public class TimingQuickTimeEventConfig : QTEConfigSOBase
{
    private const float MaxProgress = 100f;

    [Header("Notification")]
    [Min(0f)]
    public float NotificationDuration = 0f;
    public SoundKeyReference NotificationClip;

    [Header("Result Feedback")]
    public SoundKeyReference SuccessClip;
    public SoundKeyReference GreatSuccessClip;
    public SoundKeyReference FailClip;

    [Header("Needle")]
    [Min(0f)]
    public float NeedleSpeedPerSecond = 100f;
    public bool RotateClockwise = true;

    [Header("Judgement")]
    public Vector2 SuccessZoneSizeRange = new Vector2(12.5f, 25f);
    public Vector2 GreatZonePercentRange = new Vector2(15f, 35f);
    [Range(0f, MaxProgress)]
    public float SuccessZoneMinStartProgress = 15f;
    
    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (QTEType != EQTEType.Timing)
        {
            Debug.LogWarning($"[{nameof(TimingQuickTimeEventConfig)}] QTEType should be Timing.", this);
        }

        SuccessZoneSizeRange.x = Mathf.Clamp(SuccessZoneSizeRange.x, 0f, MaxProgress);
        SuccessZoneSizeRange.y = Mathf.Clamp(SuccessZoneSizeRange.y, SuccessZoneSizeRange.x, MaxProgress);
        GreatZonePercentRange.x = Mathf.Max(0f, GreatZonePercentRange.x);
        GreatZonePercentRange.y = Mathf.Max(GreatZonePercentRange.x, GreatZonePercentRange.y);
        SuccessZoneMinStartProgress = Mathf.Clamp(SuccessZoneMinStartProgress, 0f, MaxProgress);
    }
    #endif
    
}
