using _02.Scripts._02.Ingame.Tutorial.Domain;
using UnityEngine;

namespace _02.Scripts._02.Ingame.Tutorial.Component
{
    public class TutorialTrigger : MonoBehaviour
    {
        [SerializeField] private TutorialStepId _triggerStepId;
        private GameEventPublisher _publisher;

        private void Awake()
        {
            _publisher = new GameEventPublisher();
            _publisher.SetSource(this);
        }

        // UnityEvent 호출
        public void Complete(int stepId)
        {
            _publisher.TryPublish(ctx => new TutorialStepCompletedRawEvent(ctx, (TutorialStepId)stepId));
        }

        // Collider 트리거
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) Complete((int)_triggerStepId);
        }
    }
}