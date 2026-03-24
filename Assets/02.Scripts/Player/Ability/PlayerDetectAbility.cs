using UnityEngine;

public class PlayerDetectAbility : PlayerAbility
{
    [SerializeField] private Camera _camera;
    [SerializeField] private RaycastSetting _query = new(5f, ~0, QueryTriggerInteraction.Ignore);

    private PlayerTargetDetector _playerTargetDetector;
    private InteractController _currentTarget;

    public InteractController CurrentTarget => _currentTarget;

    private void Start()
    {
        _camera = Camera.main;
        _playerTargetDetector = new PlayerTargetDetector(
            new RaycastService(),
            _query);
    }

    private void Update()
    {
        InteractController nextDetectTarget =
            _playerTargetDetector.Detect(_camera.transform.position, _camera.transform.forward);

        if (ReferenceEquals(_currentTarget, nextDetectTarget))
        {
            return;
        }

        _currentTarget?.OnDetectExit();
        _currentTarget = nextDetectTarget;
        _currentTarget?.OnDetectEnter();
    }

    private void OnDisable()
    {
        ClearCurrentHoverTarget();
    }

    private void ClearCurrentHoverTarget()
    {
        if (_currentTarget == null)
        {
            return;
        }

        _currentTarget.OnDetectExit();
        _currentTarget = null;
    }
}
