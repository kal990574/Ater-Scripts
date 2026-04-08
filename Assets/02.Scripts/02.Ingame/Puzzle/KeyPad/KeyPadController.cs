using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class KeyPadController : MonoBehaviour, IPlayerPuzzleController
{
    [Header("KeyPad Puzzle")]
    [SerializeField] private string _correctCode = "0000";

    [Header("KeyPad Instance")]
    [SerializeField] private GameObject _keyPadInstancePrefab;
    [SerializeField] private Transform _spawnParentOverride;
    [SerializeField] private Vector3 _localSpawnPosition = new(0f, 0f, 0.5f);
    [SerializeField] private Vector3 _localSpawnEulerAngles = Vector3.zero;

    [Header("State")]
    [SerializeField] private bool _blockOpenAfterSuccess = true;

    [Header("Puzzle Events")] 
    [SerializeField] private UnityEvent _buttonEvent;
    [SerializeField] private UnityEvent _successEvent;
    [SerializeField] private UnityEvent _failEvent;

    private KeyPadPuzzleInstance _activeInstance;
    private KeyPadInteractable _activeInteractable;
    private bool _isSolved;
    private PlayerController _playerController;

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

        Transform spawnParent = ResolveSpawnParent();
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

    public void ConfirmActivePuzzle()
    {
        if (_activeInstance == null)
        {
            return;
        }

        _activeInstance.TryEvaluate();
    }

    public void CancelActivePuzzle()
    {
        if (_activeInstance == null)
        {
            return;
        }

        _activeInstance.Cancel();
    }

    public void HandlePuzzleSuccess(KeyPadPuzzleInstance instance)
    {
        if (_activeInstance != instance)
        {
            return;
        }

        _isSolved = true;
        _successEvent?.Invoke();
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
    }

    public void ClearActiveInstance(KeyPadPuzzleInstance instance)
    {
        if (_activeInstance != instance)
        {
            return;
        }

        ResolvePlayerController()?.ExitPuzzleMode(this);
        _activeInteractable = null;
        _activeInstance = null;
    }

    private Transform ResolveSpawnParent()
    {
        if (_spawnParentOverride != null)
        {
            return _spawnParentOverride;
        }

        Camera mainCamera = Camera.main;
        return mainCamera != null ? mainCamera.transform : transform;
    }

    private PlayerController ResolvePlayerController()
    {
        if (_playerController != null)
        {
            return _playerController;
        }

        _playerController = FindFirstObjectByType<PlayerController>();
        return _playerController;
    }
}
