using _02.Scripts.Player;
using System;
using TMPro;
using UnityEngine;

public class UI_InteractPrompt : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private bool _puzzleModeOnly;

    private IDisposable _subscription;
    private PlayerController _playerController;

    private void Awake()
    {
        _playerController = FindFirstObjectByType<PlayerController>();
        if (_puzzleModeOnly && _playerController != null)
        {
            _playerController.OnModeChanged += OnModeChanged;
        }
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

    private void OnDestroy()
    {
        if (_playerController != null)
        {
            _playerController.OnModeChanged -= OnModeChanged;
        }
    }

    private void OnPromptChanged(InteractPromptRawEvent e)
    {
        if (!e.IsVisible)
        {
            _root.SetActive(false);
            return;
        }

        bool isPuzzleMode = _playerController?.GetCurrentMode() == EPlayerInteractMode.Puzzle;
        if (_puzzleModeOnly != isPuzzleMode) return;

        _root.SetActive(true);
        _text.text = e.PromptText;
    }

    private void OnModeChanged(EPlayerInteractMode mode)
    {
        if (mode != EPlayerInteractMode.Puzzle)
        {
            _root.SetActive(false);
        }
    }
}
