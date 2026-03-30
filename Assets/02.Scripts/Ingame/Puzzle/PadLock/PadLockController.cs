using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class PadLockController : MonoBehaviour
{
    [Header("Padlock Code")]
    [SerializeField] private string _correctCode = "1111";

    [Header("Padlock Instance")]
    [SerializeField] private GameObject _padlockInstancePrefab;
    [SerializeField] private Transform _spawnParentOverride;
    [SerializeField] private Vector3 _localSpawnPosition = new(0f, 0f, 0.5f);
    [SerializeField] private Vector3 _localSpawnEulerAngles = Vector3.zero;

    [Header("State")]
    [SerializeField] private bool _blockOpenAfterSuccess = true;
    [SerializeField] private GameObject _lockVisualToDisable;

    [Header("Puzzle Events")]
    [SerializeField] private UnityEvent _successEvent;
    [SerializeField] private UnityEvent _failEvent;

    private PadLockPuzzleInstance _activeInstance;
    private bool _isSolved;
    private PlayerController _playerController;

    [ContextMenu("On")]
    public void TryOpen()
    {
        if (_blockOpenAfterSuccess && _isSolved)
        {
            return;
        }

        if (_activeInstance != null)
        {
            return;
        }

        if (_padlockInstancePrefab == null)
        {
            Debug.LogWarning($"{gameObject.name} : padlock instance prefab is not assigned", this);
            return;
        }

        Transform spawnParent = ResolveSpawnParent();
        GameObject instanceObject = Instantiate(_padlockInstancePrefab, spawnParent);
        instanceObject.transform.localPosition = _localSpawnPosition;
        instanceObject.transform.localRotation = Quaternion.Euler(_localSpawnEulerAngles);

        _activeInstance = instanceObject.GetComponent<PadLockPuzzleInstance>();
        if (_activeInstance == null)
        {
            Debug.LogWarning($"{instanceObject.name} : PadLockPuzzleInstance component is missing", instanceObject);
            Destroy(instanceObject);
            return;
        }

        _activeInstance.Initialize(this, _correctCode);
        ResolvePlayerController()?.EnterPuzzleMode(this);
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

    public void HandlePuzzleSuccess(PadLockPuzzleInstance instance)
    {
        if (_activeInstance != instance)
        {
            return;
        }

        _isSolved = true;
        _successEvent?.Invoke();

        if (_lockVisualToDisable != null)
        {
            _lockVisualToDisable.SetActive(false);
        }

        ResolvePlayerController()?.ExitPuzzleMode(this);
        _activeInstance = null;
    }

    public void HandlePuzzleFail(PadLockPuzzleInstance instance)
    {
        if (_activeInstance != instance)
        {
            return;
        }

        _failEvent?.Invoke();
    }

    public void ClearActiveInstance(PadLockPuzzleInstance instance)
    {
        if (_activeInstance == instance)
        {
            ResolvePlayerController()?.ExitPuzzleMode(this);
            _activeInstance = null;
        }
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
