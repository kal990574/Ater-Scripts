using UnityEngine;

public struct RaycastRequest
{
    public Vector3 Origin { get; }
    public Vector3 Direction { get; }
    public float Distance { get; }
    public int LayerMask { get; }
    public QueryTriggerInteraction TriggerInteraction { get; }

    public RaycastRequest(
        Vector3 origin,
        Vector3 direction,
        float distance,
        int layerMask,
        QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
    {
        Origin = origin;
        Direction = direction.normalized;
        Distance = distance;
        LayerMask = layerMask;
        TriggerInteraction = triggerInteraction;
    }
}
