using UnityEngine;

[CreateAssetMenu(fileName = "TimingConfig", menuName = "QTE/Config/Timing")]
public class TimingQuickTimeEventConfig : QTEConfigSOBase
{
    [Header("Notification")]
    [Min(0f)]
    public float NotificationDuration = 0f;
    public AudioClip NotificationClip;

    [Header("Result Feedback")]
    public AudioClip SuccessClip;
    public AudioClip GreatSuccessClip;
    public AudioClip FailClip;

    [Header("Needle")]
    [Min(0f)]
    public float NeedleSpeedPerSecond = 100f;
    public bool RotateClockwise = true;

    [Header("Judgement")]
    public Vector2 SuccessZoneSizeRange = new Vector2(12.5f, 25f);
    public Vector2 GreatZonePercentRange = new Vector2(15f, 35f);
}
