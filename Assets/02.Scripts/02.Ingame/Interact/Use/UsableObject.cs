using UnityEngine;


public class UsableObject : Interactable
{
    public string LastFailureReason { get; private set; } = string.Empty;
    public UseInteractResult LastInteractResult { get; protected set; } = UseInteractResult.None;


    public override void Interact(InteractionContext context)
    {
        if (!IsInteractActive)
        {
            FailUse(UseInteractResult.NotActive, "Interaction is not active.", context);
            return;
        }

        InteractionContext resolvedContext = context ?? InteractionContext.For((PlayerController)null, this);
        if (!TryCanUse(resolvedContext, out string failureReason))
        {
            FailUse(LastInteractResult, failureReason, resolvedContext);
            return;
        }

        LastFailureReason = string.Empty;
        if (!OnUse(resolvedContext, out string runtimeFailureReason))
        {
            if (LastInteractResult == UseInteractResult.None || LastInteractResult == UseInteractResult.Success)
            {
                LastInteractResult = UseInteractResult.InvalidConfiguration;
            }

            if (string.IsNullOrWhiteSpace(runtimeFailureReason))
            {
                runtimeFailureReason = "Use execution failed.";
            }

            FailUse(LastInteractResult, runtimeFailureReason, resolvedContext);
            return;
        }

        if (LastInteractResult == UseInteractResult.None)
        {
            LastInteractResult = UseInteractResult.Success;
        }
        OnInteractActivate();
        OnUseSucceeded(resolvedContext);
    }

    protected virtual bool CanUse(InteractionContext context, out string failureReason)
    {
        failureReason = string.Empty;
        return true;
    }

    protected virtual bool OnUse(InteractionContext context, out string failureReason)
    {
        failureReason = string.Empty;
        return true;
    }

    protected virtual void OnUseFailed(InteractionContext context, string failureReason)
    {
    }

    protected virtual void OnUseSucceeded(InteractionContext context)
    {
    }

    private bool TryCanUse(InteractionContext context, out string failureReason)
    {
        if (!CanUse(context, out failureReason))
        {
            if (LastInteractResult == UseInteractResult.None)
            {
                LastInteractResult = UseInteractResult.InvalidConfiguration;
            }

            if (string.IsNullOrWhiteSpace(failureReason))
            {
                failureReason = "Use requirements are not satisfied.";
            }

            return false;
        }

        LastInteractResult = UseInteractResult.None;
        failureReason = string.Empty;
        return true;
    }
    
    protected void SetFailureResult(UseInteractResult result)
    {
        LastInteractResult = result;
    }

    private void FailUse(UseInteractResult result, string failureReason, InteractionContext context)
    {
        LastInteractResult = result;
        LastFailureReason = failureReason ?? string.Empty;
        Debug.LogWarning($"[{GetType().Name}] {gameObject.name} interaction failed. result={LastInteractResult}, reason={LastFailureReason}", this);
        OnUseFailed(context, LastFailureReason);
    }
}
