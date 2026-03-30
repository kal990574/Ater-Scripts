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

    public IDetectable Detect(Vector3 origin, Vector3 direction)
    {
        RaycastRequest request = _query.CreateRequest(origin, direction);
        RaycastResult result = _raycastService.Cast(request);

        if (!result.Hit || result.Collider == null)
        {
            return null;
        }

        return FindTargetable(result.Collider);
    }

    private static IDetectable FindTargetable(Collider collider)
    {
        IDetectable detectable = collider.GetComponentInChildren<IDetectable>();
        if (detectable != null && detectable.CanDetect)
        {
            return detectable;
        }

        detectable = collider.GetComponentInParent<IDetectable>();
        if (detectable != null && detectable.CanDetect)
        {
            return detectable;
        }

        return null;
    }
}
