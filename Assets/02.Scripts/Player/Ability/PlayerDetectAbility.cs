using UnityEngine;

//플레이어가 타겟 오브젝트를 탐지하는 어빌리티
public class PlayerDetectAbility : PlayerAbility
{
    [SerializeField] private Camera _camera;
    [SerializeField] private RaycastSetting _query = new(5f, ~0, QueryTriggerInteraction.Ignore);
    
    private PlayerTargetDetector _playerTargetDetector;
    private IInteractTarget _currentTarget;
    
    public IInteractTarget CurrentTarget => _currentTarget;

    private void Start()
    {
        _camera = Camera.main;
        _playerTargetDetector = new PlayerTargetDetector(
            new RaycastService(),
            _query);
    }

    private void Update()
    {
        IInteractTarget nextInteractTarget = _playerTargetDetector.Detect(_camera.transform.position, _camera.transform.forward);
        if (ReferenceEquals(_currentTarget, nextInteractTarget))
        {
            return;
        }

        _currentTarget?.OnTargetDetectExit();
        _currentTarget = nextInteractTarget;
        _currentTarget?.OnTargetDetectEnter();
        if (_currentTarget != null)
        {
            Debug.Log($"감지 {_currentTarget}");
        }
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

        _currentTarget.OnTargetDetectExit();
        _currentTarget = null;
    }
}
