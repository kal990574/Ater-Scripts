using Sirenix.OdinInspector;
using UnityEngine;

public abstract class PuzzleControllerBase : MonoBehaviour, IPuzzleInputHandler
{
    [Header("Instance Transform")]
    [SerializeField, SceneObjectsOnly, Required] 
    private PlayerController _playerController;
    [SerializeField,SceneObjectsOnly, Required] 
    protected Camera _puzzleCamera;
    
    [SerializeField] protected Vector3 _localSpawnPosition = new(0f, 0f, 0.5f);
    [SerializeField] protected Vector3 _localSpawnEulerAngles = Vector3.zero;

    [Header("State")]
    [SerializeField] private bool _blockOpenAfterSuccess = true;
    [SerializeField] private GameObject _lockVisualToDisable;

    private GameEventPublisher _eventPublisher;
    protected PuzzleInteractable _activeInteractable;
    
    protected abstract EPuzzleType PuzzleType { get; }
    EPuzzleType IPuzzleInputHandler.PuzzleType => PuzzleType;
    protected abstract IPuzzleIntance ActivePuzzleInstance { get; }

    private bool _isSolved;
    public bool IsSolved => _isSolved;
    public Camera PuzzleCamera =>  _puzzleCamera != null ? _puzzleCamera : Camera.main;
    public bool HasActivePuzzle => ActivePuzzleInstance != null;

    protected virtual void Awake()
    {
        _eventPublisher = new GameEventPublisher();
        _eventPublisher.SetSource(this);
    }

    public abstract void TryOpen(PuzzleInteractable interactable);

    public void ConfirmActivePuzzle()
    {
        if (ActivePuzzleInstance == null)
        {
            return;
        }

        ActivePuzzleInstance.TryEvaluate();
    }

    public void CancelActivePuzzle()
    {
        if (ActivePuzzleInstance == null)
        {
            return;
        }

        ActivePuzzleInstance.Cancel();
    }

    protected bool CanStartPuzzle(string alreadySolvedMessage, string alreadyActiveMessage)
    {
        if (_blockOpenAfterSuccess && _isSolved)
        {
            if (!string.IsNullOrEmpty(alreadySolvedMessage))
            {
                Debug.LogWarning(alreadySolvedMessage, this);
            }

            return false;
        }

        if (ActivePuzzleInstance != null)
        {
            if (!string.IsNullOrEmpty(alreadyActiveMessage))
            {
                Debug.LogWarning(alreadyActiveMessage, this);
            }

            return false;
        }

        return true;
    }

    protected GameObject SpawnPuzzleInstance(GameObject instancePrefab, string missingPrefabMessage)
    {
        if (instancePrefab == null)
        {
            Debug.LogWarning(missingPrefabMessage, this);
            return null;
        }

        if (_puzzleCamera != null)
        {
            _puzzleCamera.gameObject.SetActive(true);
        }

        Transform spawnParent = ResolveSpawnParent(_puzzleCamera.transform);
        GameObject instanceObject = Instantiate(instancePrefab, spawnParent);
        instanceObject.transform.localPosition = _localSpawnPosition;
        instanceObject.transform.localRotation = Quaternion.Euler(_localSpawnEulerAngles);
        return instanceObject;
    }

    protected bool TryResolvePuzzleInstance<TInstance>(
        GameObject instanceObject,
        out TInstance puzzleInstance,
        string missingComponentMessage)
        where TInstance : Component, IPuzzleIntance
    {
        puzzleInstance = null;

        if (instanceObject == null)
        {
            return false;
        }

        if (instanceObject.TryGetComponent(out puzzleInstance))
        {
            return true;
        }

        Debug.LogWarning(missingComponentMessage, instanceObject);
        Destroy(instanceObject);
        ClosePuzzleSession();
        return false;
    }

    protected void PublishPuzzleResult(EPuzzleResult result)
    {
        _eventPublisher.TryPublish(
            context => new PuzzleResultRawEvent(
                context,
                PuzzleType,
                result));
    }

    protected void EnterPuzzleMode()
    {
        ResolvePlayerController()?.EnterPuzzleMode(this);
    }

    protected void HandlePuzzleSolved()
    {
        _isSolved = true;
        PublishPuzzleResult(EPuzzleResult.Success);

        if (_lockVisualToDisable != null)
        {
            _lockVisualToDisable.SetActive(false);
        }

        ClosePuzzleSession();
    }

    protected void HandlePuzzleCancelled()
    {
        PublishPuzzleResult(EPuzzleResult.Cancel);
        ClosePuzzleSession();
    }

    protected void ClosePuzzleSession()
    {
        ResolvePlayerController()?.ExitPuzzleMode(this);

        if (_puzzleCamera != null)
        {
            _puzzleCamera.gameObject.SetActive(false);
        }
    }

    protected Transform ResolveSpawnParent(Transform puzzleCamera)
    {
        if (puzzleCamera != null)
        {
            return puzzleCamera;
        }

        Debug.LogError("Puzzle camera is not assigned.");
        Camera mainCamera = Camera.main;
        return mainCamera != null ? mainCamera.transform : transform;
    }

    protected PlayerController ResolvePlayerController()
    {
        if (_playerController != null)
        {
            return _playerController;
        }

        _playerController = FindFirstObjectByType<PlayerController>();
        return _playerController;
    }
}
