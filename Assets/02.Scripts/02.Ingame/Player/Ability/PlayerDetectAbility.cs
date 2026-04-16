using UnityEngine;

namespace _02.Scripts.Player
{
    public class PlayerDetectAbility : PlayerAbility
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private RaycastSetting _query = new(5f, ~0, QueryTriggerInteraction.Ignore);

        private PlayerDetectTargetTracker _targetTracker;
        private PlayerEventPublisher _eventPublisher;

        public IDetectable CurrentTarget => _targetTracker != null ? _targetTracker.CurrentTarget : null;
        public RaycastSetting PromptQuery => _query;

        private void Start()
        {
            _camera = Camera.main;
            _targetTracker = new PlayerDetectTargetTracker(
                new PlayerTargetDetector(
                    new RaycastService(),
                    _query));
        }

        public void SetEventPublisher(PlayerEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        private void Update()
        {
            if (_camera == null)
            {
                return;
            }

            _targetTracker?.UpdateTarget(_camera.transform.position, _camera.transform.forward);
            _eventPublisher?.UpdatePrompt(_camera);
        }

        public void ResumePrompt()
        {
            _eventPublisher?.Resume();
        }

        public void ForceHidePrompt()
        {
            _eventPublisher?.HideUntilLookAway();
        }

        private void OnDisable()
        {
            ClearCurrentHoverTarget();
        }

        private void ClearCurrentHoverTarget()
        {
            _targetTracker?.Clear();
            _eventPublisher?.Clear();
        }
    }
}
