using System;
using UnityEngine;

public class PuzzleHUDManager : MonoBehaviour
{
    public static PuzzleHUDManager Instance { get; private set; }

    [SerializeField] private PuzzleHUDDataSo _data;
    [SerializeField] private PlayerController _playerController;

    public event Action<string, bool> OnHUDChanged;

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

    private void HandlePuzzleEntered(EPuzzleType type)
    {
        if (_data.BuildPuzzleHUDString(type, out string prompt))
            OnHUDChanged?.Invoke(prompt, true);
    }

    private void HandlePuzzleExited()
    {
        OnHUDChanged?.Invoke(string.Empty, false);
    }
}