using System;

namespace _02.Scripts.Player
{
    public class PlayerBlockedInputRouter
    {
        private readonly PlayerModeService _modeService;
        private readonly Action _inventoryToggleAction;

        public PlayerBlockedInputRouter(
            PlayerModeService modeService,
            Action inventoryToggleAction)
        {
            _modeService = modeService;
            _inventoryToggleAction = inventoryToggleAction;
        }

        public bool Handle(IPlayerInput input)
        {
            switch (_modeService.CurrentMode)
            {
                case EPlayerInteractMode.UI:
                    if (input.InventoryToggleInput)
                    {
                        _inventoryToggleAction?.Invoke();
                    }
                    return true;
                case EPlayerInteractMode.Puzzle:
                    if (input.ConfirmInput)
                    {
                        _modeService.ActivePuzzleInputHandler?.ConfirmActivePuzzle();
                    }

                    if (input.CancelInput)
                    {
                        _modeService.ActivePuzzleInputHandler?.CancelActivePuzzle();
                    }
                    return true;
                case EPlayerInteractMode.Cutscene:
                    return true;
                default:
                    return false;
            }
        }
    }
}
