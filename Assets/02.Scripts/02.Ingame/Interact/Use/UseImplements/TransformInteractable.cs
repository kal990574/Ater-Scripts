using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class TransformInteractable : UsableObject
{
    private enum EDestinationMode
    {
        TargetTransform = 0,
        WorldPose = 1,
        LocalPose = 2
    }

    [TabGroup("Inspector", "TransformInteractable")]
    [Required]
    [SerializeField] private Transform _transformToMove;

    [TabGroup("Inspector", "TransformInteractable")]
    [EnumToggleButtons]
    [SerializeField] private EDestinationMode _destinationMode = EDestinationMode.TargetTransform;

    [TabGroup("Inspector", "TransformInteractable")]
    [ShowIf(nameof(IsTargetTransformMode))]
    [Required]
    [SerializeField] private Transform _destinationTransform;

    [TabGroup("Inspector", "TransformInteractable")]
    [ShowIf(nameof(IsWorldPoseMode))]
    [LabelText("Position")]
    [SerializeField] private Vector3 _worldPosition;

    [TabGroup("Inspector", "TransformInteractable")]
    [ShowIf(nameof(IsWorldPoseMode))]
    [LabelText("Rotation")]
    [SerializeField] private Vector3 _worldEulerAngles;

    [TabGroup("Inspector", "TransformInteractable")]
    [ShowIf(nameof(IsLocalPoseMode))]
    [LabelText("Position")]
    [SerializeField] private Vector3 _localPosition;

    [TabGroup("Inspector", "TransformInteractable")]
    [ShowIf(nameof(IsLocalPoseMode))]
    [LabelText("Rotation")]
    [SerializeField] private Vector3 _localEulerAngles;
    
    [TabGroup("Inspector", "TransformInteractable")]
    [MinValue(0f)]
    [LabelText("Move Duration")]
    [SerializeField] private float _moveDuration = 0.5f;

    [TabGroup("Inspector", "TransformInteractable")]
    [LabelText("Move Ease")]
    [SerializeField] private Ease _moveEase = Ease.OutCubic;

    [TabGroup("Inspector", "TransformInteractable")]
    [MinValue(0f)]
    [LabelText("Rotate Duration")]
    [SerializeField] private float _rotateDuration = 0.5f;

    [TabGroup("Inspector", "TransformInteractable")]
    [LabelText("Rotate Ease")]
    [SerializeField] private Ease _rotateEase = Ease.OutCubic;

    [TabGroup("Inspector", "TransformInteractable")]
    [LabelText("Rotate Mode")]
    [SerializeField] private RotateMode _rotateMode = RotateMode.Fast;

    [TabGroup("Inspector", "TransformInteractable")]
    [ToggleLeft]
    [LabelText("Disable Interaction While Tweening")]
    [SerializeField] private bool _disableInteractionWhileTweening = true;

    [TabGroup("Inspector", "TransformInteractable")]
    [ToggleLeft]
    [LabelText("Disable Interaction After Use")]
    [SerializeField] private bool _disableInteractionAfterUse;

    [TabGroup("Inspector", "TransformInteractable")]
    [LabelText("On Move Started")]
    [SerializeField] private UnityEvent _onMoveStarted;

    [TabGroup("Inspector", "TransformInteractable")]
    [LabelText("On Move Completed")]
    [SerializeField] private UnityEvent _onMoveCompleted;

    private Sequence _sequence;

    private bool IsTargetTransformMode => _destinationMode == EDestinationMode.TargetTransform;
    private bool IsWorldPoseMode => _destinationMode == EDestinationMode.WorldPose;
    private bool IsLocalPoseMode => _destinationMode == EDestinationMode.LocalPose;

    protected override void OnAwake()
    {
        base.OnAwake();

        if (_transformToMove == null)
        {
            _transformToMove = transform;
        }
    }

    protected override void OnBeforeDestroy()
    {
        base.OnBeforeDestroy();
        KillTweens();
    }

    protected override bool CanUse(InteractionContext context, out string failureReason)
    {
        if (!ValidateConfiguration(out failureReason))
        {
            SetFailureResult(EUseInteractResult.InvalidConfiguration);
            return false;
        }

        failureReason = string.Empty;
        return true;
    }

    protected override bool OnUse(InteractionContext context, out string failureReason)
    {
        ResolveDestination(out Vector3 targetPosition, out Vector3 targetEulerAngles, out bool useLocalSpace);

        StartSequence(targetPosition, targetEulerAngles, useLocalSpace);

        if (_disableInteractionWhileTweening)
        {
            SetActivate(false);
        }

        _onMoveStarted?.Invoke();
        SetFailureResult(EUseInteractResult.Success);
        failureReason = string.Empty;
        return true;
    }

    private bool ValidateConfiguration(out string failureReason)
    {
        if (_transformToMove == null)
        {
            failureReason = "Transform to move is not assigned.";
            return false;
        }

        if (_moveDuration < 0.0f)
        {
            failureReason = "Move duration must be zero or greater.";
            return false;
        }

        if (_rotateDuration < 0.0f)
        {
            failureReason = "Rotate duration must be zero or greater.";
            return false;
        }

        if (_destinationMode == EDestinationMode.TargetTransform && _destinationTransform == null)
        {
            failureReason = "Destination transform is not assigned.";
            return false;
        }

        failureReason = string.Empty;
        return true;
    }

    private void ResolveDestination(out Vector3 targetPosition, out Vector3 targetEulerAngles, out bool useLocalSpace)
    {
        switch (_destinationMode)
        {
            case EDestinationMode.TargetTransform:
                targetPosition = _destinationTransform.position;
                targetEulerAngles = _destinationTransform.rotation.eulerAngles;
                useLocalSpace = false;
                break;

            case EDestinationMode.LocalPose:
                targetPosition = _localPosition;
                targetEulerAngles = _localEulerAngles;
                useLocalSpace = true;
                break;

            default:
                targetPosition = _worldPosition;
                targetEulerAngles = _worldEulerAngles;
                useLocalSpace = false;
                break;
        }
    }

    private void StartMoveTween(Vector3 targetPosition, bool useLocalSpace)
    {
        Tween moveTween = useLocalSpace
            ? _transformToMove.DOLocalMove(targetPosition, _moveDuration)
            : _transformToMove.DOMove(targetPosition, _moveDuration);

        moveTween.SetEase(_moveEase);

        _sequence.Join(moveTween);
    }

    private void StartRotateTween(Vector3 targetEulerAngles, bool useLocalSpace)
    {
        Tween rotateTween = useLocalSpace
            ? _transformToMove.DOLocalRotate(targetEulerAngles, _rotateDuration, _rotateMode)
            : _transformToMove.DORotate(targetEulerAngles, _rotateDuration, _rotateMode);

        rotateTween.SetEase(_rotateEase);

        _sequence.Join(rotateTween);
    }

    private void StartSequence(Vector3 targetPosition, Vector3 targetEulerAngles, bool useLocalSpace)
    {
        KillTweens(false);
        _sequence = DOTween.Sequence();
        _sequence.SetUpdate(UpdateType.Normal);

        StartMoveTween(targetPosition, useLocalSpace);
        StartRotateTween(targetEulerAngles, useLocalSpace);

        _sequence.OnComplete(() =>
        {
            _sequence = null;
            HandleSequenceFinished();
        });
        _sequence.OnKill(() =>
        {
            _sequence = null;
        });
    }

    private void HandleSequenceFinished()
    {
        if (_disableInteractionAfterUse)
        {
            SetActivate(false);
        }
        else if (_disableInteractionWhileTweening)
        {
            SetActivate(true);
        }

        _onMoveCompleted?.Invoke();
    }

    private void KillTweens()
    {
        KillTweens(true);
    }

    private void KillTweens(bool complete)
    {
        if (_sequence == null)
        {
            return;
        }

        if (_sequence.IsActive())
        {
            _sequence.Kill(complete);
        }

        _sequence = null;
    }
}
