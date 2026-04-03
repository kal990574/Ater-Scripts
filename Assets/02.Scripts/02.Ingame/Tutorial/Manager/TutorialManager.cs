using _02.Scripts._02.Ingame.Tutorial.Config;
using _02.Scripts._02.Ingame.Tutorial.Domain;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _02.Scripts._02.Ingame.Tutorial.Manager
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private TutorialConfigSO _config;

        private HashSet<TutorialStepId> _completedSteps = new();
        private TutorialStepId _currentStep = TutorialStepId.None;
        private CompositeSubscription _subscriptions;

        // UI 통신용 이벤트
        public event Action<TutorialStepEntry> OnGuideShow;
        public event Action OnGuideHide;
        public event Action<TutorialStepEntry> OnOverlayShow;
        public event Action OnOverlayHide;

        private void OnEnable()
        {
            GameEventHub hub = GameEventHub.Instance;
            if (hub == null) return;

            _subscriptions = new CompositeSubscription();
            _subscriptions.Add(hub.Subscribe<PlayerMovedRawEvent>(_ => TryDismiss(TutorialStepId.Movement)));
            _subscriptions.Add(hub.Subscribe<SonarScanStartedRawEvent>(_ => TryDismiss(TutorialStepId.Sonar)));
            _subscriptions.Add(hub.Subscribe<LidarScanStartedRawEvent>(_ => TryDismiss(TutorialStepId.Lidar)));
            _subscriptions.Add(hub.Subscribe<InteractedRawEvent>(_ => TryDismiss(TutorialStepId.Interact)));
            _subscriptions.Add(hub.Subscribe<ItemEquippedRawEvent>(_ => TryDismiss(TutorialStepId.Equip)));
            _subscriptions.Add(hub.Subscribe<InventoryToggledRawEvent>(e =>
            {
                if (e.IsOpen) TryDismiss(TutorialStepId.Inventory);
            }));
            _subscriptions.Add(hub.Subscribe<ItemDraggedRawEvent>(_ => TryDismiss(TutorialStepId.Inspect)));
            _subscriptions.Add(hub.Subscribe<ItemScrolledRawEvent>(_ => TryDismiss(TutorialStepId.Manipulate)));

            // 이벤트 트리거 발동용
            _subscriptions.Add(hub.Subscribe<LidarScanStartedRawEvent>(_ => TryShow(TutorialStepId.Lidar)));
            _subscriptions.Add(hub.Subscribe<InteractedRawEvent>(_ => TryShow(TutorialStepId.Interact)));
            _subscriptions.Add(hub.Subscribe<ItemAddedRawEvent>(_ => TryShowOnItemAdded()));
        }

        private void OnDisable()
        {
            _subscriptions?.Dispose();
        }

        private void Start()
        {
            // 게임 시작 시 Movement 가이드 표시
            TryShow(TutorialStepId.Movement);
        }

        // 가이드 표시 시도
        public void TryShow(TutorialStepId stepId)
        {
            if (_completedSteps.Contains(stepId)) return;
            if (_currentStep == stepId) return;

            // ChainFrom이 설정된 스텝은 이전 스텝 완료 필요
            TutorialStepEntry entry = _config.GetStep(stepId);
            if (entry == null) return;
            if (entry.ChainFrom != TutorialStepId.None && !_completedSteps.Contains(entry.ChainFrom)) return;

            // 현재 가이드 즉시 소멸
            if (_currentStep != TutorialStepId.None)
            {
                HideCurrent();
            }

            _currentStep = stepId;

            if (entry.IsOverlay)
                OnOverlayShow?.Invoke(entry);
            else
                OnGuideShow?.Invoke(entry);
        }

        // 소멸 조건 충족 시 호출
        private void TryDismiss(TutorialStepId stepId)
        {
            if (_currentStep != stepId) return;

            _completedSteps.Add(stepId);
            HideCurrent();
            TryShowChained(stepId);
        }

        // 체인 가이드 탐색 — 이전 스텝 소멸 후 자동 발동
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

        // 아이템 추가 시 — 첫 번째는 Equip, 두 번째 이후는 Inventory 발동
        private void TryShowOnItemAdded()
        {
            if (!_completedSteps.Contains(TutorialStepId.Equip))
            {
                TryShow(TutorialStepId.Equip);
            }
            else if (!_completedSteps.Contains(TutorialStepId.Inventory))
            {
                TryShow(TutorialStepId.Inventory);
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