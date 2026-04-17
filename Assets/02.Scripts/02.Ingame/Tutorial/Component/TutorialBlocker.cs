using _02.Scripts._02.Ingame.Tutorial.Domain;
using _02.Scripts._02.Ingame.Tutorial.Manager;
using System;
using UnityEngine;

namespace _02.Scripts._02.Ingame.Tutorial.Component
{
    public enum EBlockerAction
    {
        Deactivate,
        Activate
    }

    [Serializable]
    public class BlockerEntry
    {
        public GameObject Target;
        public TutorialStepId TriggerStep;
        public EBlockerAction Action;
    }

    public class TutorialBlocker : MonoBehaviour
    {
        [SerializeField] private TutorialManager _tutorialManager;
        [SerializeField] private BlockerEntry[] _entries;

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
            foreach (var entry in _entries)
            {
                if (entry.Target != null && entry.TriggerStep == stepId)
                    entry.Target.SetActive(entry.Action == EBlockerAction.Activate);
            }
        }
    }
}