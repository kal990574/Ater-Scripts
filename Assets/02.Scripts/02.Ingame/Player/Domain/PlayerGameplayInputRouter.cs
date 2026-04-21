using System;
using UnityEngine;

namespace _02.Scripts.Player
{
    public class PlayerGameplayInputRouter
    {
        private readonly PlayerModeService _modeService;
        private readonly Func<IDetectable> _targetResolver;
        private readonly PlayerInteractAbility _interactAbility;
        private readonly PlayerScanAbility _scanAbility;
        private readonly PlayerHandAbility _handAbility;
        private readonly Func<int, bool> _itemModeRequester;
        private readonly Func<bool> _scanModeRequester;
        private readonly Action _inventoryToggleAction;

        public PlayerGameplayInputRouter(
            PlayerModeService modeService,
            Func<IDetectable> targetResolver,
            PlayerInteractAbility interactAbility,
            PlayerScanAbility scanAbility,
            PlayerHandAbility handAbility,
            Func<int, bool> itemModeRequester,
            Func<bool> scanModeRequester,
            Action inventoryToggleAction)
        {
            _modeService = modeService;
            _targetResolver = targetResolver;
            _interactAbility = interactAbility;
            _scanAbility = scanAbility;
            _handAbility = handAbility;
            _itemModeRequester = itemModeRequester;
            _scanModeRequester = scanModeRequester;
            _inventoryToggleAction = inventoryToggleAction;
        }

        public void Handle(IPlayerInput input)
        {
            HandleInteraction(input);
            HandleModeSwitch(input);
            HandleCurrentMode(input);
        }

        private static bool IsQtePlaying()
        {
            return QTEManager.Instance != null && QTEManager.Instance.IsPlaying;
        }

        private static void HandleQte(IPlayerInput input)
        {
            if (input.InteractInput)
            {
                QTEManager.Instance?.SubmitCurrent();
            }
        }

        private void HandleInteraction(IPlayerInput input)
        {
            if (IsQtePlaying())
            {
                HandleQte(input);
                return;
            }
            
            IDetectable target = _targetResolver?.Invoke();
            if (target != null && input.InteractInput)
            {
                _interactAbility?.Interact(target);
            }
        }

        private void HandleModeSwitch(IPlayerInput input)
        {
            if (input.ModeToggleInput)
            {
                _scanModeRequester?.Invoke();
            }

            if (input.InventoryToggleInput)
            {
                _inventoryToggleAction?.Invoke();
            }

            int itemSlotIndex = input.ItemSlotInput;
            if (itemSlotIndex >= 0)
            {
                _itemModeRequester?.Invoke(itemSlotIndex);
            }
        }

        private void HandleCurrentMode(IPlayerInput input)
        {
            if (_modeService.CurrentMode == EPlayerInteractMode.Scan)
            {
                HandleScanMode(input);
            }

            float scroll = input.ScrollInput;
            if (Mathf.Approximately(scroll, 0f))
            {
                return;
            }

            if (_handAbility == null || _handAbility.InventoryCount <= 0)
            {
                return;
            }

            int direction = scroll > 0f ? 1 : -1;
            int current = _handAbility.CurrentHandIndex < 0 ? 0 : _handAbility.CurrentHandIndex;
            int nextIndex = (current + direction + _handAbility.InventoryCount) % _handAbility.InventoryCount;
            _itemModeRequester?.Invoke(nextIndex);
        }

        private void HandleScanMode(IPlayerInput input)
        {
            if (_scanAbility == null)
            {
                return;
            }

            if (input.RmbPressInput)
            {
                _scanAbility.SonarActive();
            }

            if (input.LmbPressInput)
            {
                _scanAbility.LidarScanActive();
            }

            if (input.LmbHoldInput)
            {
                _scanAbility.LidarScanUpdate();
            }

            if (input.InteractInput)
            {
                QTEManager.Instance?.SubmitCurrent();
            }

            if (input.LmbReleaseInput)
            {
                _scanAbility.LidarScanDeactive();
            }
        }
    }
}
