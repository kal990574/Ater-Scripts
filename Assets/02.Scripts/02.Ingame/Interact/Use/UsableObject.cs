
using Sirenix.OdinInspector;
using UnityEngine;


public class UsableObject : Interactable
{
    public string LastFailureReason { get; private set; } = string.Empty;
    public EUseInteractResult LastInteractResult { get; protected set; } = EUseInteractResult.None;

    [Header("Failure Notification")]
    [SerializeField] private InteractionFailSO _failMessage;
    [SerializeField] private int _failureMessageIndex = -1;



    [Button]
    public virtual void UnlockForce()
    {
        Unlock();
    }

    public virtual bool Unlock()
    {
        return true;
    }

    public override void Interact(InteractionContext context)
    {
        if (!IsInteractActive)
        {
            FailUse(EUseInteractResult.NotActive, "Interaction is not active.", context);
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
            if (LastInteractResult == EUseInteractResult.None || LastInteractResult == EUseInteractResult.Success)
            {
                LastInteractResult = EUseInteractResult.InvalidConfiguration;
            }

            if (string.IsNullOrWhiteSpace(runtimeFailureReason))
            {
                runtimeFailureReason = "Use execution failed.";
            }

            FailUse(LastInteractResult, runtimeFailureReason, resolvedContext);
            return;
        }

        if (LastInteractResult == EUseInteractResult.None)
        {
            LastInteractResult = EUseInteractResult.Success;
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
        PublishObjectInteracted(GetSuccessInteractEventType(context));
    }

    protected virtual EInteractObjectEventType GetSuccessInteractEventType(InteractionContext context)
    {
        return EInteractObjectEventType.Default;
    }

    private bool TryCanUse(InteractionContext context, out string failureReason)
    {
        if (!CanUse(context, out failureReason))
        {
            if (LastInteractResult == EUseInteractResult.None)
            {
                LastInteractResult = EUseInteractResult.InvalidConfiguration;
            }

            if (string.IsNullOrWhiteSpace(failureReason))
            {
                failureReason = "Use requirements are not satisfied.";
            }

            return false;
        }

        LastInteractResult = EUseInteractResult.None;
        failureReason = string.Empty;
        return true;
    }

    protected void SetFailureResult(EUseInteractResult result)
    {
        LastInteractResult = result;
    }

    private void FailUse(EUseInteractResult result, string failureReason, InteractionContext context)
    {
        LastInteractResult = result;
        LastFailureReason = failureReason ?? string.Empty;
        Debug.LogWarning($"[{GetType().Name}] {gameObject.name} interaction failed. result={LastInteractResult}, reason={LastFailureReason}", this);
        OnUseFailed(context, LastFailureReason);

        if (_failMessage != null && _failMessage.TryGetMessage(_failureMessageIndex, out string message))
            GameEventHub.Instance.Publish(new InteractionFailedRawEvent(default, message));
    }
}

