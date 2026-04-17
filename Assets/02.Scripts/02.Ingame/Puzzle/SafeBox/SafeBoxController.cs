using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class SafeBoxController : PuzzleControllerBase
{
    [Header("SafeBox Code")]
    [SerializeField] private List<int> _correctCode = new() { 0, 1, 2, 3 };

    [Header("SafeBox Instance")]
    [SerializeField] private GameObject _safeBoxInstancePrefab;

    [Header("Puzzle Events")]
    [SerializeField] private UnityEvent _dialSpinEvent;
    [SerializeField] private UnityEvent _successEvent;
    [SerializeField] private UnityEvent _failEvent;
    [SerializeField] private UnityEvent _resetEvent;

    private SafeBoxInstance _activeInstance;

    protected override EPuzzleType PuzzleType => EPuzzleType.SafeBox;
    protected override IPuzzleIntance ActivePuzzleInstance => _activeInstance;

    public override void TryOpen(PuzzleInteractable interactable)
    {
        if (!CanStartPuzzle(
                $"[{nameof(SafeBoxController)}] {gameObject.name} puzzle open was requested, but it is already solved.",
                $"[{nameof(SafeBoxController)}] {gameObject.name} puzzle open was requested, but another safe box puzzle is already active."))
        {
            return;
        }

        if (_correctCode == null || _correctCode.Count == 0)
        {
            Debug.LogWarning($"[{nameof(SafeBoxController)}] {gameObject.name} correct code is empty.", this);
            return;
        }

        GameObject instanceObject = SpawnPuzzleInstance(
            _safeBoxInstancePrefab,
            $"[{nameof(SafeBoxController)}] {gameObject.name} safe box instance prefab is not assigned.");

        if (instanceObject == null)
        {
            return;
        }

        if (!TryResolvePuzzleInstance(
                instanceObject,
                out _activeInstance,
                $"[{nameof(SafeBoxController)}] {instanceObject.name} is missing SafeBoxInstance."))
        {
            return;
        }
        _activeInteractable = interactable;
        _activeInstance.Initialize(this, _correctCode);
        EnterPuzzleMode();
    }

    public void HandlePuzzleSuccess(SafeBoxInstance instance)
    {
        if (_activeInstance != instance)
        {
            return;
        }

        _successEvent?.Invoke();
        HandlePuzzleSolved();
        _activeInteractable?.HandlePuzzleSolved();
        _activeInteractable = null;
        _activeInstance = null;
    }

    public void HandlePuzzleFail(SafeBoxInstance instance)
    {
        if (_activeInstance != instance)
        {
            return;
        }

        _failEvent?.Invoke();
        PublishPuzzleResult(EPuzzleResult.Fail);
    }

    public void HandlePuzzleReset(SafeBoxInstance instance)
    {
        if (_activeInstance != instance)
        {
            return;
        }

        _resetEvent?.Invoke();
    }

    public void ClearActiveInstance(SafeBoxInstance instance)
    {
        if (_activeInstance != instance)
        {
            return;
        }

        HandlePuzzleCancelled();
        _activeInteractable = null;
        _activeInstance = null;
    }

    public void OnDialSpin()
    {
        if (_activeInstance == null)
        {
            return;
        }

        _dialSpinEvent?.Invoke();
    }
}
