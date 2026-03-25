using System;
using UnityEngine;

[Serializable]
public struct RaycastSetting
{
    [SerializeField] [Min(0.01f)] private float _distance;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private QueryTriggerInteraction _triggerInteraction;

    public float Distance => _distance;
    public LayerMask LayerMask => _layerMask;
    public QueryTriggerInteraction TriggerInteraction => _triggerInteraction;

    public RaycastSetting(float distance, LayerMask layerMask, QueryTriggerInteraction triggerInteraction)
    {
        _distance = distance;
        _layerMask = layerMask;
        _triggerInteraction = triggerInteraction;
    }

    public RaycastRequest CreateRequest(Vector3 origin, Vector3 direction)
    {
        return new RaycastRequest(origin, direction, _distance, _layerMask, _triggerInteraction);
    }
}
