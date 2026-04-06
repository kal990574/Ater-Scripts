using _02.Scripts._02.Ingame.Tutorial.Config;
using _02.Scripts._02.Ingame.Tutorial.Domain;
using _02.Scripts.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _02.Scripts._02.Ingame.Tutorial.Manager
{
    public class TutorialManager : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private TutorialConfigSO _config;

        [Header("References")]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private PlayerHandAbility _playerHandAbility;
        [SerializeField] private ExamineInteraction _examineInteraction;

        private IPlayerInput _playerInput;
        private CompositeSubscription _subscriptions;
        private HashSet<TutorialStepId> _completedSteps = new();
        private TutorialStepId _currentStep = TutorialStepId.None;

        public event Action<TutorialStepEntry> OnGuideShow;
        public event Action OnGuideHide;
        public event Action<TutorialStepEntry> OnOverlayShow;
        public event Action OnOverlayHide;

        private void Start()
        {
            if (_playerController != null)
            {
                _playerInput = _playerController.Input;
            }

            if (_playerHandAbility == null)
            {
                _playerHandAbility = _playerController.GetAbility<PlayerHandAbility>();
            }

            GameEventHub hub = GameEventHub.Instance;
            if (hub != null)
            {
                _subscriptions = new CompositeSubscription();
                _subscriptions.Add(hub.Subscribe<SonarScanStartedRawEvent>(_ => TryDismiss(TutorialStepId.Sonar)));
                _subscriptions.Add(hub.Subscribe<TutorialStepCompletedRawEvent>(e => TryDismiss(e.StepId)));
            }

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryToggled += OnInventoryToggled;
                InventoryManager.Instance.OnSelectionChanged += OnSelectionChanged;
            }

            if (_playerHandAbility != null)
            {
                _playerHandAbility.OnHandSlotChanged += OnHandSlotChanged;
            }

            if (_playerController != null)
            {
                _playerController.OnModeChanged += OnModeChanged;
            }

            if (_examineInteraction != null)
            {
                _examineInteraction.OnDragChanged += OnDragChanged;
                _examineInteraction.OnScrolled += OnScrolled;
            }

            StartCoroutine(ShowFirstGuide());
        }

        private IEnumerator ShowFirstGuide()
        {
            yield return null;
            TryShow(TutorialStepId.Movement);
        }

        private void OnDestroy()
        {
            _subscriptions?.Dispose();

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryToggled -= OnInventoryToggled;
                InventoryManager.Instance.OnSelectionChanged -= OnSelectionChanged;
            }

            if (_playerHandAbility != null)
            {
                _playerHandAbility.OnHandSlotChanged -= OnHandSlotChanged;
            }

            if (_playerController != null)
            {
                _playerController.OnModeChanged -= OnModeChanged;
            }

            if (_examineInteraction != null)
            {
                _examineInteraction.OnDragChanged -= OnDragChanged;
                _examineInteraction.OnScrolled -= OnScrolled;
            }
        }

        private void Update()
        {
            if (_currentStep == TutorialStepId.Movement && _playerInput != null && _playerInput.MoveInput != Vector2.zero)
            {
                TryDismiss(TutorialStepId.Movement);
            }
        }

        private void OnInventoryToggled(bool isOpen)
        {
            if (isOpen)
            {
                TryDismiss(TutorialStepId.OpenInventory);
            }
        }

        private void OnHandSlotChanged(int index)
        {
            if (index >= 0)
            {
                TryDismiss(TutorialStepId.EquipKey);
            }
        }

        private void OnSelectionChanged(int index)
        {
            if (index >= 0)
            {
                TryDismiss(TutorialStepId.InspectNote);
            }
        }

        private void OnModeChanged(EPlayerInteractMode mode)
        {
            if (mode == EPlayerInteractMode.Scan)
            {
                TryDismiss(TutorialStepId.ScannerToggle);
            }
        }

        private void OnDragChanged(bool isDragging)
        {
            if (isDragging)
            {
                TryDismiss(TutorialStepId.RotateItem);
            }
        }

        private void OnScrolled(float delta)
        {
            TryDismiss(TutorialStepId.ZoomItem);
        }

        public void TryShow(TutorialStepId stepId)
        {
            Debug.Log($"[Tutorial] TryShow({stepId}) called");

            if (_completedSteps.Contains(stepId))
            {
                Debug.Log($"[Tutorial] {stepId} already completed");
                return;
            }

            if (_currentStep == stepId)
            {
                Debug.Log($"[Tutorial] {stepId} already current");
                return;
            }

            TutorialStepEntry entry = _config.GetStep(stepId);
            if (entry == null)
            {
                Debug.Log($"[Tutorial] {stepId} not found in config");
                return;
            }

            if (entry.ChainFrom != TutorialStepId.None && _completedSteps.Contains(entry.ChainFrom) == false)
            {
                Debug.Log($"[Tutorial] {stepId} chain prerequisite {entry.ChainFrom} not met");
                return;
            }

            if (_currentStep != TutorialStepId.None)
            {
                HideCurrent();
            }

            _currentStep = stepId;
            Debug.Log($"[Tutorial] Showing {stepId}: {entry.GuideText}");

            if (entry.IsOverlay)
            {
                OnOverlayShow?.Invoke(entry);
            }
            else
            {
                OnGuideShow?.Invoke(entry);
            }
        }

        public void TryDismiss(TutorialStepId stepId)
        {
            if (_currentStep != stepId)
            {
                return;
            }

            _completedSteps.Add(stepId);
            HideCurrent();
            TryShowChained(stepId);
        }

        private void TryShowChained(TutorialStepId completedStep)
        {
            foreach (TutorialStepEntry entry in _config.Steps)
            {
                if (entry.ChainFrom == completedStep && _completedSteps.Contains(entry.Id) == false)
                {
                    TryShow(entry.Id);
                    return;
                }
            }
        }

        private void HideCurrent()
        {
            TutorialStepEntry entry = _config.GetStep(_currentStep);
            _currentStep = TutorialStepId.None;

            if (entry != null && entry.IsOverlay)
            {
                OnOverlayHide?.Invoke();
            }
            else
            {
                OnGuideHide?.Invoke();
            }
        }
    }
}