using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UI_AchievementPopupQueue : MonoBehaviour
{
    [SerializeField] private AchievementManager _achievementManager;
    [SerializeField] private UI_AchievementPopup _popupPrefab;
    [SerializeField] private Transform _popupRoot;
    [SerializeField] private float _hiddenOffsetY = -180f;
    [SerializeField] private float _enterDuration = 0.35f;
    [SerializeField] private float _showDuration = 2.5f;
    [SerializeField] private float _exitDuration = 0.3f;
    [SerializeField] private Ease _enterEase = Ease.OutCubic;
    [SerializeField] private Ease _exitEase = Ease.InCubic;

    private readonly Queue<AchievementDefinition> _queue = new Queue<AchievementDefinition>();
    private Coroutine _playCoroutine;
    private Sequence _activeSequence;
    private UI_AchievementPopup _activePopup;

    private void OnEnable()
    {
        if (_achievementManager != null)
        {
            _achievementManager.AchievementUnlocked += OnAchievementUnlocked;
        }
    }

    private void OnDisable()
    {
        if (_achievementManager != null)
        {
            _achievementManager.AchievementUnlocked -= OnAchievementUnlocked;
        }

        StopPlayback();
    }

    private void OnAchievementUnlocked(AchievementDefinition definition, AchievementState state)
    {
        if (definition == null)
        {
            return;
        }

        _queue.Enqueue(definition);

        if (_playCoroutine == null)
        {
            _playCoroutine = StartCoroutine(CoPlayQueue());
        }
    }

    private IEnumerator CoPlayQueue()
    {
        while (_queue.Count > 0)
        {
            AchievementDefinition definition = _queue.Dequeue();

            UI_AchievementPopup popupInstance = Instantiate(_popupPrefab, _popupRoot);
            _activePopup = popupInstance;
            popupInstance.Bind(definition);

            RectTransform popupRect = popupInstance.transform as RectTransform;
            if (popupRect != null)
            {
                Vector2 shownPosition = popupRect.anchoredPosition;
                Vector2 hiddenPosition = shownPosition + new Vector2(0f, _hiddenOffsetY);

                popupRect.anchoredPosition = hiddenPosition;

                KillActiveSequence();
                _activeSequence = DOTween.Sequence()
                    .SetUpdate(true);
                _activeSequence.Append(
                    popupRect.DOAnchorPos(shownPosition, _enterDuration)
                        .SetEase(_enterEase)
                );
                _activeSequence.AppendInterval(_showDuration);
                _activeSequence.Append(
                    popupRect.DOAnchorPos(hiddenPosition, _exitDuration)
                        .SetEase(_exitEase)
                );

                yield return _activeSequence.WaitForCompletion();
                KillActiveSequence();
            }
            else
            {
                yield return new WaitForSecondsRealtime(_showDuration);
            }

            if (popupInstance != null)
            {
                Destroy(popupInstance.gameObject);
            }

            if (_activePopup == popupInstance)
            {
                _activePopup = null;
            }
        }

        _playCoroutine = null;
    }

    private void StopPlayback()
    {
        if (_playCoroutine != null)
        {
            StopCoroutine(_playCoroutine);
            _playCoroutine = null;
        }

        KillActiveSequence();

        if (_activePopup != null)
        {
            Destroy(_activePopup.gameObject);
            _activePopup = null;
        }
    }

    private void KillActiveSequence()
    {
        if (_activeSequence == null)
        {
            return;
        }

        if (_activeSequence.IsActive() == true)
        {
            _activeSequence.Kill();
        }

        _activeSequence = null;
    }
}
