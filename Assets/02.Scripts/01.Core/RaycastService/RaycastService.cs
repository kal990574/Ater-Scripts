using UnityEngine;

public class RaycastService : IRaycastService
{
    public RaycastResult Cast(in RaycastRequest request)
    {
        Ray ray = new(request.Origin, request.Direction);

        if (Physics.Raycast(
                ray,
                out RaycastHit hit,
                request.Distance,
                request.LayerMask,
                request.TriggerInteraction) == false)
        {
            return RaycastResult.Miss;
        }

        return new RaycastResult(
            true,
            hit.collider,
            hit.point,
            hit.normal,
            hit.distance);
    }
}