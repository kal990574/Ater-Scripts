using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_AchievementPopupQueue : MonoBehaviour
{
    [SerializeField] private AchievementManager _achievementManager;
    [SerializeField] private UI_AchievementPopup _popupPrefab;
    [SerializeField] private Transform _popupRoot;
    [SerializeField] private float _showDuration = 2.5f;

    private readonly Queue<AchievementDefinition> _queue = new Queue<AchievementDefinition>();
    private Coroutine _playCoroutine;

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
            popupInstance.Bind(definition);

            yield return new WaitForSeconds(_showDuration);

            if (popupInstance != null)
            {
                Destroy(popupInstance.gameObject);
            }
        }

        _playCoroutine = null;
    }
}
