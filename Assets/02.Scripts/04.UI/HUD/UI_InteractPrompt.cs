using System;
using TMPro;
using UnityEngine;

public class UI_InteractPrompt : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private bool _puzzleModeOnly;

    private IDisposable _subscription;
    private IPlayerModeProvider _modeProvider;

    private void Start()
    {
        _modeProvider = FindFirstObjectByType<PlayerController>();
    }

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
        if (!e.IsVisible)
        {
            _root.SetActive(false);
            return;
        }

        bool isPuzzleMode = _modeProvider?.GetCurrentMode() == EPlayerInteractMode.Puzzle;
        if (_puzzleModeOnly != isPuzzleMode) return;

        _root.SetActive(true);
        _text.text = e.PromptText;
    }
}
