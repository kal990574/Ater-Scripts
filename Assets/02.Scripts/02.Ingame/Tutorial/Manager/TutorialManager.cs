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
        [SerializeField] private ExamineInteraction _examineInteraction;

        private IPlayerInput _playerInput;
        private CompositeSubscription _subscriptions;
        private HashSet<TutorialStepId> _completedSteps = new();
        private TutorialStepId _currentStep = TutorialStepId.None;

        // UI 통신용 이벤트
        public event Action<TutorialStepEntry> OnGuideShow;
        public event Action OnGuideHide;
        public event Action<TutorialStepEntry> OnOverlayShow;
        public event Action OnOverlayHide;

        private void Start()
        {
            _playerInput = _playerController.Input;

            // 기존 EventBus 이벤트
            GameEventHub hub = GameEventHub.Instance;
            if (hub != null)
            {
                _subscriptions = new CompositeSubscription();
                _subscriptions.Add(hub.Subscribe<SonarScanStartedRawEvent>(_ => TryDismiss(TutorialStepId.Sonar)));
                _subscriptions.Add(hub.Subscribe<TutorialStepCompletedRawEvent>(e => TryDismiss(e.StepId)));
            }

            // 기존 Action 구독
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryToggled += OnInventoryToggled;
                InventoryManager.Instance.OnHandSlotChanged += OnHandSlotChanged;
                InventoryManager.Instance.OnSelectionChanged += OnSelectionChanged;
                InventoryManager.Instance.OnInventoryItemChanged += OnInventoryItemChanged;
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

            // 최초 가이드 표시
            StartCoroutine(ShowFirstGuide());
        }
        
        // UI 구독 대기
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
                InventoryManager.Instance.OnHandSlotChanged -= OnHandSlotChanged;
                InventoryManager.Instance.OnSelectionChanged -= OnSelectionChanged;
                InventoryManager.Instance.OnInventoryItemChanged -= OnInventoryItemChanged;
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
            if (_currentStep == TutorialStepId.Movement && _playerInput.MoveInput != Vector2.zero)
            {
                TryDismiss(TutorialStepId.Movement);
            }
        }

        // --- Action 핸들러 ---
        private void OnInventoryToggled(bool isOpen)
        {
            if (isOpen) TryDismiss(TutorialStepId.OpenInventory);
        }

        private void OnHandSlotChanged(int index)
        {
            if (index >= 0) TryDismiss(TutorialStepId.EquipKey);
        }

        private void OnSelectionChanged(int index)
        {
            if (index >= 0) TryDismiss(TutorialStepId.InspectNote);
        }

        private void OnInventoryItemChanged()
        {
            if (_currentStep == TutorialStepId.FindKey)
                TryDismiss(TutorialStepId.FindKey);
            else if (_currentStep == TutorialStepId.FindNote)
                TryDismiss(TutorialStepId.FindNote);
        }

        private void OnModeChanged(EPlayerInteractMode mode)
        {
            if (mode == EPlayerInteractMode.Scan) TryDismiss(TutorialStepId.ScannerToggle);
        }

        private void OnDragChanged(bool isDragging)
        {
            if (isDragging) TryDismiss(TutorialStepId.RotateItem);
        }

        private void OnScrolled(float delta)
        {
            TryDismiss(TutorialStepId.ZoomItem);
        }

        // --- 핵심 로직 ---
        public void TryShow(TutorialStepId stepId)
        {
            Debug.Log($"[Tutorial] TryShow({stepId}) called");

            if (_completedSteps.Contains(stepId)) { Debug.Log($"[Tutorial] {stepId} already completed"); return; }
            if (_currentStep == stepId) { Debug.Log($"[Tutorial] {stepId} already current"); return; }

            TutorialStepEntry entry = _config.GetStep(stepId);
            if (entry == null) { Debug.Log($"[Tutorial] {stepId} not found in config"); return; }
            if (entry.ChainFrom != TutorialStepId.None && !_completedSteps.Contains(entry.ChainFrom)) { Debug.Log($"[Tutorial] {stepId} chain prerequisite {entry.ChainFrom} not met"); return; }

            if (_currentStep != TutorialStepId.None)
            {
                HideCurrent();
            }

            _currentStep = stepId;
            Debug.Log($"[Tutorial] Showing {stepId}: {entry.GuideText}");

            if (entry.IsOverlay)
                OnOverlayShow?.Invoke(entry);
            else
                OnGuideShow?.Invoke(entry);
        }

        public void TryDismiss(TutorialStepId stepId)
        {
            if (_currentStep != stepId) return;

            _completedSteps.Add(stepId);
            HideCurrent();
            TryShowChained(stepId);
        }

        private void TryShowChained(TutorialStepId completedStep)
        {
            foreach (TutorialStepEntry entry in _config.Steps)
            {
                if (entry.ChainFrom == completedStep && !_completedSteps.Contains(entry.Id))
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
                OnOverlayHide?.Invoke();
            else
                OnGuideHide?.Invoke();
        }
    }
}