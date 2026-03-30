using UnityEngine;
using UnityEngine.UI;

public class UI_CircleTimingQTE : MonoBehaviour, ITimingQuickTimeEventView
{
    private const float MaxProgress = 100f;
    private const float FullCircleAngle = 360f;

    [Header("Required References")]
    [SerializeField] private Image _successZoneImage;
    [SerializeField] private Image _greatZoneImage;
    [SerializeField] private Transform _judgeZoneTransform;
    [SerializeField] private Transform _needleTransform;
    [SerializeField] private CanvasGroup _canvasGroup;

    public void Show()
    {
        gameObject.SetActive(true);

        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
    }

    public void Hide()
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        gameObject.SetActive(false);
    }

    public void ResetView()
    {
        UpdateView(0f, 0f, 0f, 0f);
    }

    public void UpdateView(float successZoneSizeProgress, float greatZonePercent, float judgeZoneStartProgress, float needleProgress)
    {
        ChangeSuccessSize(successZoneSizeProgress);
        ChangeGreatRate(greatZonePercent);
        SetJudgeZone(judgeZoneStartProgress);
        RotateNeedle(needleProgress);
    }

    private void ChangeSuccessSize(float progress)
    {
        float normalized = Mathf.Clamp01(progress / MaxProgress);
        _successZoneImage.fillAmount = normalized;
    }

    private void ChangeGreatRate(float percent)
    {
        float normalized = Mathf.Clamp01(percent / 100f);
        _greatZoneImage.fillAmount = _successZoneImage.fillAmount * normalized;
    }

    private void SetJudgeZone(float startProgress)
    {
        float angle = ConvertProgressToAngle(startProgress);
        _judgeZoneTransform.localRotation = Quaternion.Euler(0f, 0f, -angle);
    }

    private void RotateNeedle(float progress)
    {
        float angle = ConvertProgressToAngle(progress);
        _needleTransform.localRotation = Quaternion.Euler(0f, 0f, -angle);
    }

    private float ConvertProgressToAngle(float progress)
    {
        float normalized = Mathf.Clamp01(progress / MaxProgress);
        return normalized * FullCircleAngle;
    }
}
