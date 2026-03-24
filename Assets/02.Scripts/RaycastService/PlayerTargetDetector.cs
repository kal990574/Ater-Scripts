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

    public IInteractTarget Detect(Vector3 origin, Vector3 direction)
    {
        RaycastRequest request = _query.CreateRequest(origin, direction);
        RaycastResult result = _raycastService.Cast(request);

        if (result.Hit == false || result.Collider == null)
        {
            return null;
        }

        return FindTargetable(result.Collider);
    }

    private static IInteractTarget FindTargetable(Collider collider)
    {
        MonoBehaviour[] behaviours = collider.GetComponentsInParent<MonoBehaviour>(true);
        for (int i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i] is IInteractTarget hoverable)
            {
                return hoverable;
            }
        }

        return null;
    }
}
