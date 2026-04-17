using UnityEngine;
using TMPro;
using DG.Tweening;
using _02.Scripts._02.Ingame.Tutorial.Config;
using _02.Scripts._02.Ingame.Tutorial.Manager;

public class TutorialGuideUI : MonoBehaviour
{
    [SerializeField] private TutorialManager _tutorialManager;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _guideText;
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private float _typingSpeed = 0.03f;
    [SerializeField] private SoundKeyReference _completeSoundKey;

    private Sequence _sequence;
    private Tween _typingTween;
    private bool _isShowing;

    private void OnEnable()
    {
        _tutorialManager.OnGuideShow += HandleShow;
        _tutorialManager.OnGuideHide += HandleHide;

        _canvasGroup.alpha = _isShowing ? 1f : 0f;
    }

    private void OnDisable()
    {
        _tutorialManager.OnGuideShow -= HandleShow;
        _tutorialManager.OnGuideHide -= HandleHide;
    }

    private void OnDestroy()
    {
        _sequence?.Kill();
        _typingTween?.Kill();
    }

    private void HandleShow(TutorialStepEntry entry)
    {
        _isShowing = true;
        _sequence?.Kill();

        _sequence = DOTween.Sequence();

        if (_canvasGroup.alpha > 0f)
            _sequence.Append(_canvasGroup.DOFade(0f, _fadeDuration));

        _sequence.AppendCallback(() =>
        {
            _guideText.text = entry.GuideText;
            _guideText.ForceMeshUpdate();
            _guideText.maxVisibleCharacters = 0;
        });

        // 페이드인
        _sequence.Append(_canvasGroup.DOFade(1f, _fadeDuration));

        // 사운드 + 타이핑 효과
        _sequence.AppendCallback(() =>
        {
            if (!string.IsNullOrEmpty(_completeSoundKey))
                SoundManager.Instance?.PlaySFX2D(_completeSoundKey);

            _typingTween?.Kill();
            int totalChars = _guideText.textInfo.characterCount;
            _typingTween = DOTween.To(
                () => _guideText.maxVisibleCharacters,
                x => _guideText.maxVisibleCharacters = x,
                totalChars,
                totalChars * _typingSpeed
            ).SetEase(Ease.Linear).SetUpdate(true);
        });

        _sequence.SetUpdate(true);
    }

    private void HandleHide()
    {
        _isShowing = false;
        _sequence?.Kill();

        _sequence = DOTween.Sequence();
        _sequence.Append(_canvasGroup.DOFade(0f, _fadeDuration));
        _sequence.SetUpdate(true);
    }
}