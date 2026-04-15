﻿using Sirenix.OdinInspector;
using UnityEngine;

[DisallowMultipleComponent]
public class ButtonDoorInteractable : DoorInteractable
{
    [TabGroup("Inspector", "ButtonDoorInteractable")]
    [MinValue(1)]
    [LabelText("Required Press Count")]
    [SerializeField] private int _requiredPressCount = 1;

    [TabGroup("Inspector", "ButtonDoorInteractable")]
    [ReadOnly]
    [LabelText("Current Press Count")]
    [SerializeField] private int _currentPressCount;

    [TabGroup("Inspector", "ButtonDoorInteractable")]
    [Button(ButtonSizes.Medium)]
    public void NotifyButtonPressed()
    {
        if (IsOpen)
        {
            return;
        }

        _currentPressCount++;

        Debug.Log($"[{nameof(ButtonDoorInteractable)}] {gameObject.name} button pressed. current={_currentPressCount}/{_requiredPressCount}", this);

        if (_currentPressCount >= _requiredPressCount)
        {
            TryOpenByButtonCount();
        }
    }

    protected override bool CanUse(InteractionContext context, out string failureReason)
    {
        if (_currentPressCount < _requiredPressCount)
        {
            SetFailureResult(EUseInteractResult.Locked);
            failureReason = "The door is locked. More buttons must be pressed.";
            return false;
        }

        return base.CanUse(context, out failureReason);
    }

    private void TryOpenByButtonCount()
    {
        if (!IsUnlocked)
        {
            bool unlockSucceeded = Unlock();
            if (!unlockSucceeded)
            {
                Debug.LogWarning($"[{nameof(ButtonDoorInteractable)}] {gameObject.name} failed to unlock.", this);
                return;
            }
        }

        if (IsOpen)
        {
            return;
        }

        OpenDoor();
    }
}
