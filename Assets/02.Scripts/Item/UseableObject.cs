using Unity.VisualScripting;
using UnityEngine;


public class UseableObject : InteractableObject
{
    private IUseCondition[] _useConditions;

    
    private void Awake()
    {
        _useConditions = GetComponentsInChildren<IUseCondition>();
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
        
        _isInteractActive = false;
    }

    public bool CanUse()
    {
        if (_useConditions == null || _useConditions.Length == 0)
        {
            return true;
        }
        Debug.Log($"Condition Check {_useConditions.Length}");
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
}
