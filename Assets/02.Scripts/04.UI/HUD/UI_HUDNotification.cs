using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UI_HUDNotification : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _displayDuration = 1.0f;

    private IDisposable _subscription;
    private Coroutine _hideCoroutine;

    private void OnEnable()
    {
        _subscription = GameEventHub.Instance.Subscribe<InteractionFailedRawEvent>(OnInteractionFailed);
    }

    private void OnDisable()
    {
        _subscription?.Dispose();
        _subscription = null;
    }

    private void OnInteractionFailed(InteractionFailedRawEvent e)
    {
        if(_hideCoroutine != null) StopCoroutine(_hideCoroutine);
        _text.text = e.Message;
        _root.SetActive(true);
        _hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(_displayDuration);
        _root.SetActive(false);
    }
}