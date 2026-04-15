using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class PadLockController : PuzzleControllerBase
{
    [Header("Padlock Code")]
    [SerializeField] private string _correctCode = "1111";

    [Header("Padlock Instance")]
    [SerializeField] private GameObject _padlockInstancePrefab;

    [Header("Puzzle Events")]
    [SerializeField] private UnityEvent _spinEvnet;
    [SerializeField] private UnityEvent _successEvent;
    [SerializeField] private UnityEvent _failEvent;

    private PadLockPuzzleInstance _activeInstance;

    protected override EPuzzleType PuzzleType => EPuzzleType.PadLock;
    protected override IPuzzleIntance ActivePuzzleInstance => _activeInstance;

    public override void TryOpen(PuzzleInteractable interactable)
    {
        if (!CanStartPuzzle(null, null))
        {
            return;
        }

        GameObject instanceObject = SpawnPuzzleInstance(
            _padlockInstancePrefab,
            $"{gameObject.name} : padlock instance prefab is not assigned");

        if (instanceObject == null)
        {
            return;
        }

        if (!TryResolvePuzzleInstance(
                instanceObject,
                out _activeInstance,
                $"{instanceObject.name} : PadLockPuzzleInstance component is missing"))
        {
            return;
        }

        _activeInstance.Initialize(this, _correctCode);
        EnterPuzzleMode();
    }

    public void HandlePuzzleSuccess(PadLockPuzzleInstance instance)
    {
        if (_activeInstance != instance)
        {
            return;
        }

        _successEvent?.Invoke();
        HandlePuzzleSolved();
        _activeInstance = null;
    }

    public void HandlePuzzleFail(PadLockPuzzleInstance instance)
    {
        if (_activeInstance != instance)
        {
            return;
        }

        _failEvent?.Invoke();
        PublishPuzzleResult(EPuzzleResult.Fail);
    }

    public void ClearActiveInstance(PadLockPuzzleInstance instance)
    {
        if (_activeInstance != instance)
        {
            return;
        }

        HandlePuzzleCancelled();
        _activeInstance = null;
    }

    public void OnSpin()
    {
        _spinEvnet?.Invoke();
    }
}
