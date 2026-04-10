using _02.Scripts._02.Ingame.Tutorial.Domain;
using _02.Scripts._02.Ingame.Tutorial.Manager;
using UnityEngine;

namespace _02.Scripts._02.Ingame.Tutorial.Component
{
    public class TutorialBlocker : MonoBehaviour
    {
        [SerializeField] private TutorialManager _tutorialManager;
        [SerializeField] private TutorialStepId _unlockAfterStep;

        private void OnEnable()
        {
            if (_tutorialManager != null)
                _tutorialManager.OnStepCompleted += OnStepCompleted;
        }

        private void OnDisable()
        {
            if (_tutorialManager != null)
                _tutorialManager.OnStepCompleted -= OnStepCompleted;
        }

        private void OnStepCompleted(TutorialStepId stepId)
        {
            if (stepId == _unlockAfterStep)
                gameObject.SetActive(false);
        }
    }
}