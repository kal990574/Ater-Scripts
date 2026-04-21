using System;
using UnityEngine;

public class HoverPromptManager : MonoBehaviour
{
    public static HoverPromptManager Instance { get; private set; }

    [SerializeField] private HoverPromptDataSO _promptData;
    [SerializeField] private PlayerController _playerController;

    public event Action<string, bool> OnPromptChanged;

    private int[] _cachedIds;
    private bool _isSuspended;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        if (_playerController == null)
            _playerController = FindFirstObjectByType<PlayerController>();

        if (_playerController != null)
        {
            _playerController.OnPuzzleModeEntered += HandlePuzzleEntered;
            _playerController.OnPuzzleModeExited += HandlePuzzleExited;
        }
    }

    private void OnDisable()
    {
        if (_playerController != null)
        {
            _playerController.OnPuzzleModeEntered -= HandlePuzzleEntered;
            _playerController.OnPuzzleModeExited -= HandlePuzzleExited;
        }
    }

    public void ShowPrompt(int[] promptIds)
    {
        _cachedIds = promptIds;
        if (_isSuspended) return;
        EmitPrompt(promptIds);
    }

    public void HidePrompt()
    {
        _cachedIds = null;
        OnPromptChanged?.Invoke(string.Empty, false);
    }

    private void HandlePuzzleEntered(EPuzzleType type)
    {
        _isSuspended = true;
        OnPromptChanged?.Invoke(string.Empty, false);
    }

    private void HandlePuzzleExited()
    {
        _isSuspended = false;
        if (_cachedIds != null)
            EmitPrompt(_cachedIds);
    }

    private void EmitPrompt(int[] promptIds)
    {
        string text = _promptData.BuildPromptString(promptIds);
        OnPromptChanged?.Invoke(text, !string.IsNullOrEmpty(text));
    }
}