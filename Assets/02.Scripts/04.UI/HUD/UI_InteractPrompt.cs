using System;
using TMPro;
using UnityEngine;

public class UI_InteractPrompt : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TextMeshProUGUI _text;

    private IDisposable _subscription;

    private void OnEnable()
    {
        _subscription = GameEventHub.Instance.Subscribe<InteractPromptRawEvent>(OnPromptChanged);
    }

    private void OnDisable()
    {
        _subscription?.Dispose();
        _subscription = null;
    }

    private void OnPromptChanged(InteractPromptRawEvent e)
    {
        _root.SetActive(e.IsVisible);

        if (e.IsVisible)
        {
            _text.text = e.PromptText;
        }
    }
}
