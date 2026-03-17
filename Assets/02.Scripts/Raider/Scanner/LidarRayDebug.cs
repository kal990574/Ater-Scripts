using UnityEngine;

[ExecuteAlways]
public class LidarRayDebug : MonoBehaviour
{
    [SerializeField] private LidarController _controller;
    [SerializeField] private LidarRaycastAbility raycastAbility;
    
    [SerializeField] private bool drawInPlayMode = true;
    [SerializeField] private bool drawInSceneView = true;
    [SerializeField] private float pointRadius = 0.05f;
    [SerializeField] private bool drawConeOutline = true;
    
    //레이케스트 그리기

    private void DrawSingleDebugRay(Vector3 origin, Vector3 direction)
    {
        bool isHit = Physics.Raycast(
            origin,
            direction,
            out RaycastHit hit,
            _controller.RayDistance,
            _controller.HitMask,
            QueryTriggerInteraction.Ignore);

        if (isHit == true)
        {
            Debug.DrawRay(origin, direction * hit.distance, Color.red);
            return;
        }

        Debug.DrawRay(origin, direction * _controller.RayDistance, Color.green);
    }
    
    
    //아웃라인 그리기
    private void OnDrawGizmos()
    {
        if (_controller == null)
        {
            return;
        }

        if (drawInSceneView == false)
        {
            return;
        }

        if (raycastAbility == null)
        {
            return;
        }

        Vector3 origin = _controller.StartPos;

        foreach (Vector3 direction in raycastAbility.EnumerateRayDirections())
        {
            DrawSingleGizmoRay(origin, direction);
        }

        if (drawConeOutline == true)
        {
            DrawConeOutline(origin, raycastAbility);
        }
    }

    private void DrawSingleGizmoRay(Vector3 origin, Vector3 direction)
    {
        bool isHit = Physics.Raycast(
            origin,
            direction,
            out RaycastHit hit,
            _controller.RayDistance,
            _controller.HitMask,
            QueryTriggerInteraction.Ignore);

        if (isHit == true)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, hit.point);
            Gizmos.DrawSphere(hit.point, pointRadius);
            return;
        }

        Vector3 endPoint = origin + direction * _controller.RayDistance;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, endPoint);
        Gizmos.DrawSphere(endPoint, pointRadius);
    }

    private void DrawConeOutline(Vector3 origin, LidarRaycastAbility raycastAbility)
    {
        int outlineSegments = Mathf.Max(12, _controller.RaysPerRing);

        Gizmos.color = Color.yellow;

        Vector3 previousPoint = Vector3.zero;
        bool hasPreviousPoint = false;

        foreach (Vector3 direction in raycastAbility.EnumerateOutlineDirections(outlineSegments))
        {
            Vector3 point = origin + direction * _controller.RayDistance;

            Gizmos.DrawLine(origin, point);

            if (hasPreviousPoint == true)
            {
                Gizmos.DrawLine(previousPoint, point);
            }

            previousPoint = point;
            hasPreviousPoint = true;
        }
    }
}