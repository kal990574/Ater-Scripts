using UnityEngine;

namespace _02.Scripts.Player
{
    public class PlayerDetectAbility : PlayerAbility
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private RaycastSetting _query = new(5f, ~0, QueryTriggerInteraction.Ignore);

        private PlayerDetectTargetTracker _targetTracker;

        public IDetectable CurrentTarget => _targetTracker != null ? _targetTracker.CurrentTarget : null;

        private void Start()
        {
            _camera = Camera.main;
            _targetTracker = new PlayerDetectTargetTracker(
                new PlayerTargetDetector(
                    new RaycastService(),
                    _query));
        }


        private void Update()
        {
            if (_camera == null)
            {
                return;
            }
            _targetTracker?.UpdateTarget(_camera.transform.position, _camera.transform.forward);
        }

        private void OnDisable()
        {
            _targetTracker?.Clear();
        }

    }
}
