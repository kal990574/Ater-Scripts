using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class BoardInteractable : UsableObject
{
    [Header("References")]
    [SerializeField] private RuntimeView _runtimeView;
    [SerializeField] private ScannableObject _scannableObject;
    [SerializeField] private Animator _animator;

    [Header("Animation")]
    [SerializeField] private string _openAnimationStateName = "BoardOpen";

    [Header("State Keys")]
    [SerializeField] private string _openStateKey = "is_open";

    [Header("Events")]
    [SerializeField] private UnityEvent _onInteractionFailed;
    [SerializeField] private UnityEvent _onOpened;

    private void Awake()
    {
        if (_runtimeView == null)
        {
            _runtimeView = GetComponent<RuntimeView>();
        }

        if (_scannableObject == null)
        {
            _scannableObject = GetComponent<ScannableObject>();
            if (_scannableObject == null)
            {
                _scannableObject = GetComponentInChildren<ScannableObject>();
            }
        }

        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
    }

    protected override bool CanUse(InteractionContext context, out string failureReason)
    {
        if (!ValidateConfiguration(out failureReason))
        {
            SetFailureResult(UseInteractResult.InvalidConfiguration);
            return false;
        }

        if (GetState(_openStateKey))
        {
            SetFailureResult(UseInteractResult.AlreadyOpen);
            failureReason = "The board is already open.";
            return false;
        }

        if (!_scannableObject.IsProgressComplete)
        {
            SetFailureResult(UseInteractResult.NotScanned);
            failureReason = "The board cannot be opened before scan completion.";
            return false;
        }

        failureReason = string.Empty;
        return true;
    }

    protected override bool OnUse(InteractionContext context, out string failureReason)
    {
        SetState(_openStateKey, true);

        if (_animator != null && !string.IsNullOrWhiteSpace(_openAnimationStateName))
        {
            _animator.Play(_openAnimationStateName);
        }
        else
        {
            Debug.LogWarning($"[{nameof(BoardInteractable)}] {gameObject.name} is missing Animator or animation state name. animationState={_openAnimationStateName}", this);
        }

        // TODO: Play board open SFX via SoundManager.

        Debug.Log($"[{nameof(BoardInteractable)}] {gameObject.name} opened successfully. animationState={_openAnimationStateName}", this);
        _onOpened?.Invoke();
        SetActivate(false);
        SetFailureResult(UseInteractResult.Success);
        failureReason = string.Empty;
        return true;
    }

    protected override void OnUseFailed(InteractionContext context, string failureReason)
    {
        _onInteractionFailed?.Invoke();
    }

    private bool ValidateConfiguration(out string failureReason)
    {
        if (_scannableObject == null)
        {
            failureReason = "ScannableObject reference is missing.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(_openStateKey))
        {
            failureReason = "Open state key is not configured.";
            return false;
        }

        if (ResolveRuntimeData()?.State == null)
        {
            failureReason = "RuntimeData.State is not available.";
            return false;
        }

        failureReason = string.Empty;
        return true;
    }

    private bool GetState(string key)
    {
        RuntimeData runtimeData = ResolveRuntimeData();
        if (runtimeData?.State == null || string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        return runtimeData.State.GetBool(key);
    }

    private void SetState(string key, bool value)
    {
        if (_runtimeView != null)
        {
            _runtimeView.SetBoolState(key, value);
            return;
        }

        RuntimeData runtimeData = ResolveRuntimeData();
        if (runtimeData?.State == null)
        {
            return;
        }

        runtimeData.State.SetBool(key, value);
        RefreshRuntimeView();
    }

    private RuntimeData ResolveRuntimeData()
    {
        if (_runtimeView != null && _runtimeView.RuntimeData != null)
        {
            return _runtimeView.RuntimeData;
        }

        return RuntimeData;
    }
}
