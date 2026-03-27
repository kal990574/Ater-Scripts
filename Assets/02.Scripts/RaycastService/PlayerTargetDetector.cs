using UnityEngine;

public class PlayerTargetDetector
{
    private readonly IRaycastService _raycastService;
    private readonly RaycastSetting _query;

    public PlayerTargetDetector(IRaycastService raycastService, RaycastSetting query)
    {
        _raycastService = raycastService;
        _query = query;
    }

    public IDetectableObject Detect(Vector3 origin, Vector3 direction)
    {
        RaycastRequest request = _query.CreateRequest(origin, direction);
        RaycastResult result = _raycastService.Cast(request);

        if (!result.Hit || result.Collider == null)
        {
            return null;
        }

        return FindTargetable(result.Collider);
    }

    private static IDetectableObject FindTargetable(Collider collider)
    {
        IDetectableObject detectableObject = collider.GetComponentInChildren<IDetectableObject>();
        if (detectableObject != null && detectableObject.CanDetect)
        {
            return detectableObject;
        }

        detectableObject = collider.GetComponentInParent<IDetectableObject>();
        if (detectableObject != null && detectableObject.CanDetect)
        {
            return detectableObject;
        }

        return null;
    }
}
