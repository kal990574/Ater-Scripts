using Sirenix.OdinInspector;
using UnityEngine;

public class UsableObject : Interactable
{
    public string LastFailureReason { get; private set; } = string.Empty;
    public EUseInteractResult LastInteractResult { get; protected set; } = EUseInteractResult.None;

    [TabGroup("Inspector", "UsableObject")]
    [SerializeField] private InteractionFailSO _failMessage;

    [TabGroup("Inspector", "UsableObject")]
    [LabelText("Message Index")]
    [SerializeField] private int _failureMessageIndex = -1;

    [TabGroup("Inspector", "UsableObject")] 
    [SerializeField]
    protected EAfterInteract _afterUse;
    
    [TabGroup("Inspector", "UsableObject")]
    [Button(ButtonSizes.Medium)]
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
        InteractionContext resolvedContext = context ?? InteractionContext.CreateEmpty(this);
        UseInteractionOutcome preconditionOutcome = EvaluatePreconditions(resolvedContext);
        if (!preconditionOutcome.IsSuccess)
        {
            ApplyFailure(preconditionOutcome, resolvedContext);
            return;
        }

        LastFailureReason = string.Empty;
        UseInteractionOutcome useOutcome = ExecuteUse(resolvedContext);
        if (!useOutcome.IsSuccess)
        {
            ApplyFailure(useOutcome, resolvedContext);
            return;
        }

        LastInteractResult = useOutcome.Result;
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
        ApplyAfterUse();
    }

    protected virtual EInteractObjectEventType GetSuccessInteractEventType(InteractionContext context)
    {
        return EInteractObjectEventType.Default;
    }

    private UseInteractionOutcome EvaluatePreconditions(InteractionContext context)
    {
        if (IsInteractComplete)
        {
            return UseInteractionOutcome.Fail(EUseInteractResult.AlreadyCompleted, "Interaction is already completed.");
        }

        if (!IsInteractActive)
        {
            return UseInteractionOutcome.Fail(EUseInteractResult.NotActive, "Interaction is not active.");
        }

        if (CanUse(context, out string failureReason))
        {
            LastInteractResult = EUseInteractResult.None;
            return UseInteractionOutcome.Success();
        }

        EUseInteractResult result = ResolveFailureResult(EUseInteractResult.InvalidConfiguration);
        if (string.IsNullOrWhiteSpace(failureReason))
        {
            failureReason = "Use requirements are not satisfied.";
        }

        return UseInteractionOutcome.Fail(result, failureReason);
    }

    private UseInteractionOutcome ExecuteUse(InteractionContext context)
    {
        if (OnUse(context, out string failureReason))
        {
            EUseInteractResult result = ResolveFailureResult(EUseInteractResult.Success);
            return new UseInteractionOutcome(result, string.Empty);
        }

        EUseInteractResult failureResult = ResolveFailureResult(EUseInteractResult.InvalidConfiguration);
        if (string.IsNullOrWhiteSpace(failureReason))
        {
            failureReason = "Use execution failed.";
        }

        return UseInteractionOutcome.Fail(failureResult, failureReason);
    }

    protected void SetFailureResult(EUseInteractResult result)
    {
        LastInteractResult = result;
    }

    protected void ApplyAfterUse()
    {
        switch (_afterUse)
        {
            case EAfterInteract.None:
                break;
            case EAfterInteract.Deactive:
                SetActivate(false);
                break;
            case EAfterInteract.Disable:
                gameObject.SetActive(false);
                break;
            case EAfterInteract.Destroy:
                Destroy(gameObject);
                break;
        }
    }

    private EUseInteractResult ResolveFailureResult(EUseInteractResult defaultResult)
    {
        if (LastInteractResult == EUseInteractResult.None)
        {
            LastInteractResult = defaultResult;
        }

        return LastInteractResult;
    }

    private void ApplyFailure(UseInteractionOutcome outcome, InteractionContext context)
    {
        LastInteractResult = outcome.Result;
        LastFailureReason = outcome.Reason ?? string.Empty;
        Debug.LogWarning($"[{GetType().Name}] {gameObject.name} interaction failed. result={LastInteractResult}, reason={LastFailureReason}", this);
        OnUseFailed(context, LastFailureReason);
        InteractionFailureNotifier.Notify(_failMessage, _failureMessageIndex);
    }
}

