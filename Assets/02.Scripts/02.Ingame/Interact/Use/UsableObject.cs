using Unity.VisualScripting;
using UnityEngine;


public class UsableObject : Interactable
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
        
        if (TryGetHub(out GameEventHub hub) == false)
        {
            Debug.LogWarning("[PickupEventEmitter] GameEventHub가 존재하지 않습니다.");
            return;
        }
        
        if (RuntimeData == null)
        {
            Debug.LogWarning($"[{nameof(UsableObject)}] RuntimeData is missing. Event publish skipped.", this);
            return;
        }

        GameEventContext eventContext = CreateContext();
        UseInteractEvent gameInteractEvent = new UseInteractEvent(
            eventContext, 
            RuntimeData.InstanceId);

        hub.Publish(in gameInteractEvent);
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
