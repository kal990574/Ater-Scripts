using InspectorGadgets.Attributes;
using UnityEngine;
using UnityEngine.Events;

public class VisibleToMainCameraEvent : MonoBehaviour
{
    private const int GizmoArcSegmentCount = 24;

    [Header("Event")]
    [SerializeField]
    private UnityEvent onVisible;

    [Header("References")]
    [SerializeField]
    private Transform player;

    [SerializeField]
    private Transform targetPoint;

    [SerializeField, Readonly]
    private Renderer _renderer;

    [SerializeField, Readonly]
    private Camera _mainCamera;

    [SerializeField, Readonly]
    private bool _hasTriggered;

    [Header("View Condition")]
    [SerializeField, Range(0.0f, 1.0f)]
    private float requiredViewDot = 0.85f;

    [SerializeField]
    private bool useCameraForwardInsteadOfPlayerForward = false;

    [SerializeField]
    private bool checkLineOfSight = true;

    [SerializeField]
    private LayerMask obstacleMask = ~0;

    private void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _mainCamera = Camera.main;

        if (targetPoint == null)
        {
            targetPoint = transform;
        }
    }

    private void Update()
    {
        if (_hasTriggered)
        {
            return;
        }

        if (CanTrigger())
        {
            _hasTriggered = true;
            onVisible?.Invoke();
        }
    }

    private bool CanTrigger()
    {
        if (_mainCamera == null || _renderer == null || player == null || targetPoint == null)
        {
            return false;
        }

        if (!IsVisibleFromMainCamera())
        {
            return false;
        }

        if (!IsPlayerFacingTarget())
        {
            return false;
        }

        if (checkLineOfSight && !HasLineOfSight())
        {
            return false;
        }

        return true;
    }

    private bool IsVisibleFromMainCamera()
    {
        Vector3 viewportPoint = _mainCamera.WorldToViewportPoint(targetPoint.position);

        if (viewportPoint.z <= 0.0f)
        {
            return false;
        }

        if (viewportPoint.x < 0.0f || viewportPoint.x > 1.0f)
        {
            return false;
        }

        if (viewportPoint.y < 0.0f || viewportPoint.y > 1.0f)
        {
            return false;
        }

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_mainCamera);

        return GeometryUtility.TestPlanesAABB(planes, _renderer.bounds);
    }

    private bool IsPlayerFacingTarget()
    {
        Vector3 origin = player.position;
        Vector3 toTarget = targetPoint.position - origin;
        toTarget.y = 0.0f;

        if (toTarget.sqrMagnitude <= 0.0001f)
        {
            return true;
        }

        toTarget.Normalize();

        Vector3 forward = useCameraForwardInsteadOfPlayerForward ? _mainCamera.transform.forward : player.forward;
        forward.y = 0.0f;
        forward.Normalize();

        float dot = Vector3.Dot(forward, toTarget);

        return dot >= requiredViewDot;
    }

    private bool HasLineOfSight()
    {
        Vector3 start = _mainCamera.transform.position;
        Vector3 end = targetPoint.position;
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        if (distance <= 0.0001f)
        {
            return true;
        }

        direction.Normalize();

        if (Physics.Raycast(start, direction, out RaycastHit hit, distance, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform == targetPoint || hit.transform.IsChildOf(transform))
            {
                return true;
            }

            return false;
        }

        return true;
    }

    public void ResetTrigger()
    {
        _hasTriggered = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null || targetPoint == null)
        {
            return;
        }

        Camera gizmoCamera = _mainCamera != null ? _mainCamera : Camera.main;

        DrawViewRangeGizmo(gizmoCamera);
        DrawSightLineGizmo(gizmoCamera);
    }

    private void DrawViewRangeGizmo(Camera gizmoCamera)
    {
        Vector3 origin = player.position;
        Vector3 forward = useCameraForwardInsteadOfPlayerForward && gizmoCamera != null
            ? gizmoCamera.transform.forward
            : player.forward;

        forward.y = 0.0f;
        if (forward.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        forward.Normalize();

        Vector3 targetOffset = targetPoint.position - origin;
        targetOffset.y = 0.0f;
        float radius = Mathf.Max(1.0f, targetOffset.magnitude);
        float halfAngle = Mathf.Acos(Mathf.Clamp(requiredViewDot, -1.0f, 1.0f)) * Mathf.Rad2Deg;

        Vector3 leftDirection = Quaternion.AngleAxis(-halfAngle, Vector3.up) * forward;
        Vector3 rightDirection = Quaternion.AngleAxis(halfAngle, Vector3.up) * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + leftDirection * radius);
        Gizmos.DrawLine(origin, origin + rightDirection * radius);

        Vector3 previousPoint = origin + leftDirection * radius;
        for (int i = 1; i <= GizmoArcSegmentCount; i++)
        {
            float t = (float)i / GizmoArcSegmentCount;
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * forward;
            Vector3 nextPoint = origin + direction * radius;
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }

    private void DrawSightLineGizmo(Camera gizmoCamera)
    {
        Gizmos.color = CanTriggerInEditorPreview(gizmoCamera) ? Color.green : Color.red;
        Gizmos.DrawLine(player.position, targetPoint.position);

        if (gizmoCamera == null)
        {
            return;
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(gizmoCamera.transform.position, targetPoint.position);
    }

    private bool CanTriggerInEditorPreview(Camera gizmoCamera)
    {
        if (player == null || targetPoint == null)
        {
            return false;
        }

        Vector3 toTarget = targetPoint.position - player.position;
        toTarget.y = 0.0f;
        if (toTarget.sqrMagnitude <= 0.0001f)
        {
            return true;
        }

        Vector3 forward = useCameraForwardInsteadOfPlayerForward && gizmoCamera != null
            ? gizmoCamera.transform.forward
            : player.forward;
        forward.y = 0.0f;

        if (forward.sqrMagnitude <= 0.0001f)
        {
            return false;
        }

        toTarget.Normalize();
        forward.Normalize();

        return Vector3.Dot(forward, toTarget) >= requiredViewDot;
    }
}
