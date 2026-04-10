using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class PadLockController : PuzzleControllerBase
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
    [SerializeField] private UnityEvent _spinEvnet;
    [SerializeField] private UnityEvent _successEvent;
    [SerializeField] private UnityEvent _failEvent;

    private PadLockPuzzleInstance _activeInstance;
    private bool _isSolved;
    
    protected override EPuzzleType PuzzleType => EPuzzleType.KeyPad;
    protected override IPuzzleIntance ActivePuzzleInstance => _activeInstance;

    public bool IsSolved => _isSolved;
    public bool HasActivePuzzle => _activeInstance != null;

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

        Transform spawnParent = ResolveSpawnParent(_spawnParentOverride);
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

    
    public void HandlePuzzleSuccess(PadLockPuzzleInstance instance)
    {
        if (_activeInstance != instance)
        {
            return;
        }

        _isSolved = true;
        _successEvent?.Invoke();
        PublishPuzzleResult(EPuzzleResult.Success);

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
        PublishPuzzleResult(EPuzzleResult.Fail);
    }

    public void ClearActiveInstance(PadLockPuzzleInstance instance)
    {
        if (_activeInstance == instance)
        {
            PublishPuzzleResult(EPuzzleResult.Cancel);

            ResolvePlayerController()?.ExitPuzzleMode(this);
            _activeInstance = null;
        }
    }
    
    public void OnSpin()
    {
        _spinEvnet?.Invoke();
    }
}
