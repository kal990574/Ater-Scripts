using UnityEngine;

//플레이어가 타겟 오브젝트를 탐지하는 어빌리티
public class PlayerDetectAbility : PlayerAbility
{
    [SerializeField] private Camera _camera;
    [SerializeField] private InteractRaycastConfigSO interactConfig;
 
    private PlayerTargetDetector _playerTargetDetector;
    private IInteractTarget _currentTarget;

    private void Start()
    {
        _camera = Camera.main;
        _playerTargetDetector = new PlayerTargetDetector(
            new RaycastService(),
            interactConfig.Query);
    }

    private void Update()
    {
        if (_owner.InteractMode != PlayerInteractMode.Item)
        {
            ClearCurrentHoverTarget();
            return;
        }

        IInteractTarget nextInteractTarget = _playerTargetDetector.Detect(_camera.transform.position, _camera.transform.forward);
        if (ReferenceEquals(_currentTarget, nextInteractTarget))
        {
            return;
        }

        _currentTarget?.OnTargetDetectExit();
        _currentTarget = nextInteractTarget;
        _currentTarget?.OnTargetDetectEnter();
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
