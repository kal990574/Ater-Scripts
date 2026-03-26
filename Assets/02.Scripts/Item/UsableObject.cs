using Unity.VisualScripting;
using UnityEngine;


public class UsableObject : InteractableObject
{
    private IUseCondition[] _useConditions;
    private IUseAction[] _useActions;

    
    private void Awake()
    {
        _useConditions = GetComponentsInChildren<IUseCondition>();
        _useActions = GetComponentsInChildren<IUseAction>();
    }

    public override void Interact(UseContext context)
    {
        if (!_isInteractActive)
        {
            Debug.Log($"{gameObject.name} : interaction is not active");
            return;
        }

        UseContext resolvedContext = context ?? UseContext.For(gameObject, this);
        if (!CanUse(resolvedContext))
        {
            Debug.Log($"{gameObject.name} : use conditions are not satisfied");
            return;
        }

        Debug.Log($"{gameObject.name} : used");
        ExecuteActions(resolvedContext);
        OnInteractActivate();
        
        _isInteractActive = false;
    }

    public bool CanUse(UseContext context)
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

            if (!useCondition.CanUse(context))
            {
                return false;
            }
        }

        return true;
    }

    private void ExecuteActions(UseContext context)
    {
        if (_useActions == null || _useActions.Length == 0)
        {
            return;
        }

        foreach (IUseAction useAction in _useActions)
        {
            useAction?.Execute(context);
        }
    }
}
