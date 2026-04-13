using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class KeyPadController : PuzzleControllerBase
{
    [Header("KeyPad Puzzle")]
    [SerializeField] private string _correctCode = "0000";

    [Header("KeyPad Instance")]
    [SerializeField] private GameObject _keyPadInstancePrefab;
   

    [Header("State")]
    [SerializeField] private bool _blockOpenAfterSuccess = true;

    [Header("Puzzle Events")] 
    [SerializeField] private UnityEvent _buttonEvent;
    [SerializeField] private UnityEvent _successEvent;
    [SerializeField] private UnityEvent _failEvent;

    private KeyPadPuzzleInstance _activeInstance;
    private KeyPadInteractable _activeInteractable;
    private bool _isSolved;
    protected override EPuzzleType PuzzleType => EPuzzleType.KeyPad;
    protected override IPuzzleIntance ActivePuzzleInstance => _activeInstance;

    public bool IsSolved => _isSolved;
    public bool HasActivePuzzle => _activeInstance != null;

    public void TryOpen(KeyPadInteractable interactable)
    {
        if (_blockOpenAfterSuccess && _isSolved)
        {
            Debug.LogWarning($"[{nameof(KeyPadController)}] {gameObject.name} puzzle open was requested, but it is already solved.", this);
            return;
        }

        if (_activeInstance != null)
        {
            Debug.LogWarning($"[{nameof(KeyPadController)}] {gameObject.name} puzzle open was requested, but another keypad puzzle is already active.", this);
            return;
        }

        if (_keyPadInstancePrefab == null)
        {
            Debug.LogWarning($"[{nameof(KeyPadController)}] {gameObject.name} keyPad instance prefab is not assigned.", this);
            return;
        }
        
        _puzzleCamera.gameObject.SetActive(true);
        Transform spawnParent = ResolveSpawnParent(_puzzleCamera);
        GameObject instanceObject = Instantiate(_keyPadInstancePrefab, spawnParent);
        instanceObject.transform.localPosition = _localSpawnPosition;
        instanceObject.transform.localRotation = Quaternion.Euler(_localSpawnEulerAngles);

        _activeInstance = instanceObject.GetComponent<KeyPadPuzzleInstance>();
        if (_activeInstance == null)
        {
            Debug.LogWarning($"[{nameof(KeyPadController)}] {instanceObject.name} is missing KeyPadPuzzleInstance.", instanceObject);
            Destroy(instanceObject);
            return;
        }

        _activeInteractable = interactable;
        _activeInstance.Initialize(this, _correctCode);
        ResolvePlayerController()?.EnterPuzzleMode(this);
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

        _isSolved = true;
        _successEvent?.Invoke();
        PublishPuzzleResult(EPuzzleResult.Success);

        _activeInteractable?.HandlePuzzleSolved();

        ResolvePlayerController()?.ExitPuzzleMode(this);
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

        PublishPuzzleResult(EPuzzleResult.Cancel);

        ResolvePlayerController()?.ExitPuzzleMode(this);
        _puzzleCamera.gameObject.SetActive(false);
        _activeInteractable = null;
        _activeInstance = null;
    }
}
