using UnityEngine;
using TMPro;
using System.Collections;
using _02.Scripts._02.Ingame.Tutorial.Config;
using _02.Scripts._02.Ingame.Tutorial.Manager;

public class TutorialGuideUI : MonoBehaviour
{
    [SerializeField] private TutorialManager _tutorialManager;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _guideText;
    [SerializeField] private float _fadeDuration = 0.5f;

    private Coroutine _fadeCoroutine;

    private bool _isShowing;

    private void OnEnable()
    {
        _tutorialManager.OnGuideShow += HandleShow;
        _tutorialManager.OnGuideHide += HandleHide;

        if (_isShowing)
            _canvasGroup.alpha = 1f;
        else
            _canvasGroup.alpha = 0f;
    }

    private void OnDisable()
    {
        _tutorialManager.OnGuideShow -= HandleShow;
        _tutorialManager.OnGuideHide -= HandleHide;
    }

    private void HandleShow(TutorialStepEntry entry)
    {
        _guideText.text = entry.GuideText;
        _isShowing = true;
        StartFade(1f);
    }

    private void HandleHide()
    {
        _isShowing = false;
        StartFade(0f);
    }

    private void StartFade(float target)
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeRoutine(target));
    }

    private IEnumerator FadeRoutine(float target)
    {
        float start = _canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(start, target, elapsed / _fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = target;
        _fadeCoroutine = null;
    }
}