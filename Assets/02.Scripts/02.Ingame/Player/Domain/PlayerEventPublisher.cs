using UnityEngine;

namespace _02.Scripts.Player
{
    public class PlayerEventPublisher
    {
        private readonly GameEventPublisher _eventPublisher;
        private readonly RaycastSetting _query;

        private IDetectable _currentPromptTarget;
        private string _lastPublishedDescription = string.Empty;
        private bool _suppressPromptUntilLookAway;

        public PlayerEventPublisher(Object source, RaycastSetting query)
        {
            _query = query;
            _eventPublisher = new GameEventPublisher();
            _eventPublisher.SetSource(source);
        }

        public void UpdatePrompt(Camera camera)
        {
            if (camera == null)
            {
                return;
            }

            IDetectable promptTarget = ResolvePromptTarget(camera);

            if (_suppressPromptUntilLookAway)
            {
                if (!ReferenceEquals(_currentPromptTarget, promptTarget))
                {
                    _suppressPromptUntilLookAway = false;
                }
                else
                {
                    return;
                }
            }

            string description = promptTarget?.HoverDescription ?? string.Empty;
            bool changed = !ReferenceEquals(_currentPromptTarget, promptTarget)
                || description != _lastPublishedDescription;

            if (!changed)
            {
                return;
            }

            _currentPromptTarget = promptTarget;
            _lastPublishedDescription = description;

            bool isVisible = promptTarget != null && !string.IsNullOrEmpty(description);
            Publish(isVisible, description);
        }

        public void Resume()
        {
            _suppressPromptUntilLookAway = false;
        }

        public void HideUntilLookAway()
        {
            _suppressPromptUntilLookAway = true;
            _lastPublishedDescription = string.Empty;
            Publish(false, string.Empty);
        }

        public void Clear()
        {
            _currentPromptTarget = null;
            _lastPublishedDescription = string.Empty;
            _suppressPromptUntilLookAway = false;
            Publish(false, string.Empty);
        }

        private IDetectable ResolvePromptTarget(Camera camera)
        {
            if (!Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, _query.Distance))
            {
                return null;
            }

            return hit.collider.GetComponentInParent<IDetectable>();
        }

        private void Publish(bool isVisible, string description)
        {
            _eventPublisher.TryPublish(ctx => new InteractPromptRawEvent(ctx, isVisible, description));
        }
    }
}
