using UnityEngine;

public abstract class PuzzleControllerBase : MonoBehaviour, IPuzzleInputHandler
{
    [SerializeField] private PlayerController _playerController;

    private GameEventPublisher _eventPublisher;

    protected abstract IPuzzleIntance ActivePuzzleInstance { get; }
    protected abstract EPuzzleType PuzzleType { get; }

    protected virtual void Awake()
    {
        _eventPublisher = new GameEventPublisher();
        _eventPublisher.SetSource(this);
    }

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

    protected void PublishPuzzleResult(EPuzzleResult result)
    {
        _eventPublisher.TryPublish(
            context => new PuzzleResultRawEvent(
                context,
                PuzzleType,
                result));
    }

    protected Transform ResolveSpawnParent(Transform spawnParentOverride)
    {
        if (spawnParentOverride != null)
        {
            return spawnParentOverride;
        }

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