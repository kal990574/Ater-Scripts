using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class KeyPadController : PuzzleControllerBase
{
    [Header("KeyPad Puzzle")]
    [SerializeField] private string _correctCode = "0000";

    [Header("KeyPad Instance")]
    [SerializeField] private GameObject _keyPadInstancePrefab;

    [Header("Puzzle Events")]
    [SerializeField] private UnityEvent _buttonEvent;
    [SerializeField] private UnityEvent _successEvent;
    [SerializeField] private UnityEvent _failEvent;

    private KeyPadPuzzleInstance _activeInstance;
    

    protected override EPuzzleType PuzzleType => EPuzzleType.KeyPad;
    protected override IPuzzleIntance ActivePuzzleInstance => _activeInstance;

    public override void TryOpen(PuzzleInteractable interactable)
    {
        if (!CanStartPuzzle(
                $"[{nameof(KeyPadController)}] {gameObject.name} puzzle open was requested, but it is already solved.",
                $"[{nameof(KeyPadController)}] {gameObject.name} puzzle open was requested, but another keypad puzzle is already active."))
        {
            return;
        }

        GameObject instanceObject = SpawnPuzzleInstance(
            _keyPadInstancePrefab,
            $"[{nameof(KeyPadController)}] {gameObject.name} keyPad instance prefab is not assigned.");

        if (instanceObject == null)
        {
            return;
        }

        if (!TryResolvePuzzleInstance(
                instanceObject,
                out _activeInstance,
                $"[{nameof(KeyPadController)}] {instanceObject.name} is missing KeyPadPuzzleInstance."))
        {
            return;
        }

        _activeInteractable = interactable;
        _activeInstance.Initialize(this, _correctCode);
        EnterPuzzleMode();
    }

    public void ButtonClick()
    {
        if (_activeInstance == null)
        {
            return;
        }

        _buttonEvent?.Invoke();
    }

    public void HandlePuzzleSuccess(KeyPadPuzzleInstance instance)
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

    public void HandlePuzzleFail(KeyPadPuzzleInstance instance)
    {
        if (_activeInstance != instance)
        {
            return;
        }

        _failEvent?.Invoke();
        PublishPuzzleResult(EPuzzleResult.Fail);
    }

    public void ClearActiveInstance(KeyPadPuzzleInstance instance)
    {
        if (_activeInstance != instance)
        {
            return;
        }

        HandlePuzzleCancelled();
        _activeInteractable = null;
        _activeInstance = null;
    }
}
