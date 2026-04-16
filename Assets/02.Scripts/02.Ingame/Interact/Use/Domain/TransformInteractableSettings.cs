using DG.Tweening;
using UnityEngine;

public readonly struct TransformInteractableSettings
{
    public TransformInteractableSettings(
        Transform transformToMove,
        Transform destinationTransform,
        TransformInteractable.EDestinationMode destinationMode,
        Vector3 worldPosition,
        Vector3 worldEulerAngles,
        Vector3 localPosition,
        Vector3 localEulerAngles,
        float moveDuration,
        Ease moveEase,
        float rotateDuration,
        Ease rotateEase,
        RotateMode rotateMode)
    {
        TransformToMove = transformToMove;
        DestinationTransform = destinationTransform;
        DestinationMode = destinationMode;
        WorldPosition = worldPosition;
        WorldEulerAngles = worldEulerAngles;
        LocalPosition = localPosition;
        LocalEulerAngles = localEulerAngles;
        MoveDuration = moveDuration;
        MoveEase = moveEase;
        RotateDuration = rotateDuration;
        RotateEase = rotateEase;
        RotateMode = rotateMode;
    }

    public Transform TransformToMove { get; }
    public Transform DestinationTransform { get; }
    public TransformInteractable.EDestinationMode DestinationMode { get; }
    public Vector3 WorldPosition { get; }
    public Vector3 WorldEulerAngles { get; }
    public Vector3 LocalPosition { get; }
    public Vector3 LocalEulerAngles { get; }
    public float MoveDuration { get; }
    public Ease MoveEase { get; }
    public float RotateDuration { get; }
    public Ease RotateEase { get; }
    public RotateMode RotateMode { get; }
}
