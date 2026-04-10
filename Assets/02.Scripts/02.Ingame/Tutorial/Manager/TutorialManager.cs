using _02.Scripts._02.Ingame.Tutorial.Config;
using _02.Scripts._02.Ingame.Tutorial.Domain;
using _02.Scripts.Core;
using _02.Scripts.Core.Domain;
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
        private HashSet<TutorialStepId> _earlyCompletedActions = new();
        private TutorialStepId _currentStep = TutorialStepId.None;
        private int _currentStepIndex = -1;

        public event Action<TutorialStepEntry> OnGuideShow;
        public event Action OnGuideHide;
        public event Action<TutorialStepId> OnStepCompleted;

        private void Start()
        {
            if (_config == null)
            {
                Debug.LogError("[Tutorial] TutorialConfigSO not assigned", this);
                enabled = false;
                return;
            }

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

            StartCoroutine(StartFirstStep());
        }

        private IEnumerator StartFirstStep()
        {
            yield return null;
            ShowNext();
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

        // ─── 콜백 ───

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
            if (index < 0 || _playerHandAbility == null) return;

            var instanceId = _playerHandAbility.CurrentHandItemInstanceId;
            if (string.IsNullOrEmpty(instanceId)) return;

            var itemData = RuntimeInstanceManager.Instance?.GetItemInstance(instanceId);
            if (itemData != null && itemData.ItemId == 1) // CH0 열쇠
            {
                TryDismiss(TutorialStepId.EquipKey);
            }
        }

        private void OnSelectionChanged(int index)
        {
            if (index < 0) return;

            var instanceId = InventoryManager.Instance.GetInventoryItemInstanceIdAt(index);
            if (string.IsNullOrEmpty(instanceId)) return;

            var itemData = RuntimeInstanceManager.Instance?.GetItemInstance(instanceId);
            if (itemData != null && itemData.ItemId == 2) // CH0 쪽지
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

        // ─── 핵심 로직 ───

        private void ShowNext()
        {
            _currentStepIndex++;

            if (_currentStepIndex >= _config.Steps.Count)
            {
                Debug.Log("[Tutorial] All steps completed, transitioning to next chapter");
                var gameManager = Managers.Get<IGameManager>();
                if (gameManager == null)
                {
                    Debug.LogError("[Tutorial] GameManager not found");
                    return;
                }
                gameManager.CompleteChapter();
                return;
            }

            TutorialStepEntry entry = _config.Steps[_currentStepIndex];
            TryShow(entry.Id);
        }

        public void TryShow(TutorialStepId stepId)
        {
            Debug.Log($"[Tutorial] TryShow({stepId}) called");

            if (_completedSteps.Contains(stepId))
            {
                Debug.Log($"[Tutorial] {stepId} already completed, skipping to next");
                ShowNext();
                return;
            }

            TutorialStepEntry entry = _config.GetStep(stepId);
            if (entry == null)
            {
                Debug.Log($"[Tutorial] {stepId} not found in config");
                return;
            }

            // 선행 완료된 액션이면 자동 스킵
            if (_earlyCompletedActions.Contains(stepId))
            {
                Debug.Log($"[Tutorial] {stepId} already done early, auto-skipping");
                _earlyCompletedActions.Remove(stepId);
                _completedSteps.Add(stepId);
                OnStepCompleted?.Invoke(stepId);
                ShowNext();
                return;
            }

            _currentStep = stepId;
            Debug.Log($"[Tutorial] Showing {stepId}: {entry.GuideText}");
            OnGuideShow?.Invoke(entry);
        }

        public void TryDismiss(TutorialStepId stepId)
        {
            if (_currentStep != stepId)
            {
                _earlyCompletedActions.Add(stepId);
                return;
            }

            _completedSteps.Add(stepId);
            HideCurrent();
            OnStepCompleted?.Invoke(stepId);
            ShowNext();
        }

        private void HideCurrent()
        {
            _currentStep = TutorialStepId.None;
            OnGuideHide?.Invoke();
        }
    }
}