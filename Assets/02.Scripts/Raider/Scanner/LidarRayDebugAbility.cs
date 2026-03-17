using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteAlways]
public class LidarRayDebugAbility : LidarAbility
{
    // [SerializeField] private Transform _originPos;
    // [Header("Cone Settings")]
    // [SerializeField] private float rayDistance = 10.0f;
    // [SerializeField] private float coneAngle = 45.0f;
    // [SerializeField] private int ringCount = 4;
    // [SerializeField] private int raysPerRing = 12;
    // [SerializeField] private Vector3 originOffset = new Vector3(0.0f, 1.0f, 0.0f);
    // [SerializeField] private LayerMask hitMask = ~0;
    [Title("Reference")] 
    [SerializeField] private LidarController _controller;
    
    [Title("Settings")]
    [SerializeField] private bool drawInPlayMode = true;
    [SerializeField] private bool drawInSceneView = true;
    [SerializeField] private bool includeCenterRay = true;
    [SerializeField] private float pointRadius = 0.05f;

    private void Update()
    {
        if (_controller == null)
        {
            return;
        }
        
        if (drawInPlayMode == false)
        {
            return;
        }

        DrawDebugRays();
    }

    private void DrawDebugRays()
    {
        Vector3 origin = _controller.StartPos ;

        if (includeCenterRay == true)
        {
            DrawSingleRay(origin, transform.forward);
        }

        for (int ringIndex = 1; ringIndex <= _controller.RingCount; ringIndex++)
        {
            float ringT = (float)ringIndex / _controller.RingCount;
            float currentAngle = _controller.ConeAngle * ringT;

            for (int i = 0; i < _controller.RaysPerRing; i++)
            {
                float yaw = (360.0f / _controller.RaysPerRing) * i;
                Vector3 direction = GetConeDirection(currentAngle, yaw);
                DrawSingleRay(origin, direction);
            }
        }
    }

    private void DrawSingleRay(Vector3 origin, Vector3 direction)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hit, _controller.RayDistance, _controller.HitMask, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(origin, direction * hit.distance, Color.red);
        }
        else
        {
            Debug.DrawRay(origin, direction * _controller.RayDistance, Color.green);
        }
    }

    private Vector3 GetConeDirection(float angleFromForward, float yawAroundForward)
    {
        Vector3 localDirection = Quaternion.Euler(angleFromForward, 0.0f, 0.0f) * Vector3.forward;
        localDirection = Quaternion.AngleAxis(yawAroundForward, Vector3.forward) * localDirection;

        Vector3 worldDirection =
            transform.rotation *
            localDirection.normalized;

        return worldDirection;
    }

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

        Vector3 origin = _controller.StartPos;

        if (includeCenterRay == true)
        {
            DrawGizmoRay(origin, transform.forward);
        }

        for (int ringIndex = 1; ringIndex <= _controller.RingCount; ringIndex++)
        {
            float ringT = (float)ringIndex / _controller.RingCount;
            float currentAngle = _controller.ConeAngle * ringT;

            for (int i = 0; i < _controller.RaysPerRing; i++)
            {
                float yaw = (360.0f / _controller.RaysPerRing) * i;
                Vector3 direction = GetConeDirection(currentAngle, yaw);
                DrawGizmoRay(origin, direction);
            }
        }

        DrawConeOutline(origin);
    }

    private void DrawGizmoRay(Vector3 origin, Vector3 direction)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hit, _controller.RayDistance, _controller.HitMask, QueryTriggerInteraction.Ignore))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, hit.point);
            Gizmos.DrawSphere(hit.point, pointRadius);
        }
        else
        {
            Vector3 endPoint = origin + direction * _controller.RayDistance;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(origin, endPoint);
            Gizmos.DrawSphere(endPoint, pointRadius);
        }
    }

    private void DrawConeOutline(Vector3 origin)
    {
        int outlineSegments = Mathf.Max(12, _controller.RaysPerRing);
        float outerAngle = _controller.ConeAngle;

        Vector3 previousPoint = Vector3.zero;
        bool hasPreviousPoint = false;

        Gizmos.color = Color.yellow;

        for (int i = 0; i <= outlineSegments; i++)
        {
            float yaw = (360.0f / outlineSegments) * i;
            Vector3 direction = GetConeDirection(outerAngle, yaw);
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