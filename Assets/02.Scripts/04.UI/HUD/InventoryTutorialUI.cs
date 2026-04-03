using UnityEngine;
using TMPro;
using System.Collections;
using _02.Scripts._02.Ingame.Tutorial.Config;
using _02.Scripts._02.Ingame.Tutorial.Manager;

public class InventoryTutorialUI : MonoBehaviour
{
    [SerializeField] private TutorialManager _tutorialManager;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _overlayText;
    [SerializeField] private float _fadeDuration = 0.3f;

    private Coroutine _fadeCoroutine;

    private void OnEnable()
    {
        _canvasGroup.alpha = 0f;
        _tutorialManager.OnOverlayShow += HandleShow;
        _tutorialManager.OnOverlayHide += HandleHide;
    }

    private void OnDisable()
    {
        _tutorialManager.OnOverlayShow -= HandleShow;
        _tutorialManager.OnOverlayHide -= HandleHide;
    }

    private void HandleShow(TutorialStepEntry entry)
    {
        _overlayText.text = entry.GuideText;
        StartFade(1f);
    }

    private void HandleHide()
    {
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