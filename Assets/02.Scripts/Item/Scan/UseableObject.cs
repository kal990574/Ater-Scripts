using Unity.VisualScripting;
using UnityEngine;

public interface IUseCondition
{
    bool CanUse(UseableObject useableObject);
}

public class UseableObject : InteractableObject
{
    [SerializeField] private bool DeleteRequiredItemAfterInteract = false;
    private IUseCondition[] _useConditions;

    private void Awake()
    {
        _useConditions = GetComponents<IUseCondition>();
    }

    public override void Interact()
    {
        if (!_isInteractActive)
        {
            Debug.Log($"{gameObject.name} : interaction is not active");
            return;
        }

        if (!CanUse())
        {
            Debug.Log($"{gameObject.name} : use conditions are not satisfied");
            return;
        }

        Debug.Log($"{gameObject.name} : used");
        OnInteractActivate();
        TryDeleteRequiredHandItem();
        
        _isInteractActive = false;
    }

    public bool CanUse()
    {
        if (_useConditions == null || _useConditions.Length == 0)
        {
            return true;
        }

        foreach (IUseCondition useCondition in _useConditions)
        {
            if (useCondition == null)
            {
                continue;
            }

            if (!useCondition.CanUse(this))
            {
                return false;
            }
        }

        return true;
    }

    private void TryDeleteRequiredHandItem()
    {
        if (!DeleteRequiredItemAfterInteract)
        {
            return;
        }

        if (!HasRequireHandItemCondition())
        {
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError($"[{nameof(UseableObject)}] {nameof(InventoryManager)}.Instance is null.", this);
            return;
        }

        InventoryManager.Instance.RemoveCurrentHandItem();
    }

    private bool HasRequireHandItemCondition()
    {
        if (_useConditions == null || _useConditions.Length == 0)
        {
            return false;
        }

        foreach (IUseCondition useCondition in _useConditions)
        {
            if (useCondition is HandItemCondition)
            {
                return true;
            }
        }

        return false;
    }
}
