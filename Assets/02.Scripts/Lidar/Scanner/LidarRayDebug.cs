using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LidarRayDebug : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private LidarController _controller;
    [SerializeField] private LidarRaycastAbility _raycastAbility;

    [Header("Debug")]
    [SerializeField] private bool _activateDebug;
    [Min(0.001f)]
    [SerializeField] private float _pointRadius = 0.05f;

    private void OnDrawGizmos()
    {
        if (CanDraw() == false || _activateDebug == false)
        {
            return;
        }

        Vector3 origin = _controller.StartPos;
        if (Application.isPlaying == false || _controller.IsOnScan == false)
        {
            _raycastAbility.Scan();
        }

        DrawRayResultsInPlayMode(origin);
        DrawConeOutline(origin);
    }

    private bool CanDraw()
    {
        return _controller != null && _raycastAbility != null;
    }

    private void DrawRayResultsInPlayMode(Vector3 origin)
    {
        IReadOnlyList<LidarRayData> rayResults = _raycastAbility.RayResults;
        if (rayResults == null || rayResults.Count == 0)
        {
            return;
        }

        for (int i = 0; i < rayResults.Count; i++)
        {
            DrawSingleRayResult(origin, rayResults[i]);
        }
    }

    private void DrawSingleRayResult(Vector3 origin, LidarRayData rayData)
    {
        if (rayData.IsHit)
        {
            Gizmos.color = rayData.IsValidTarget ? Color.green : Color.yellow;
            Gizmos.DrawLine(origin, rayData.EndPoint);
            Gizmos.DrawSphere(rayData.EndPoint, _pointRadius);
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, rayData.EndPoint);
        Gizmos.DrawSphere(rayData.EndPoint, _pointRadius);
    }

    private void DrawConeOutline(Vector3 origin)
    {
        int outlineSegments = Mathf.Max(12, _controller.RaysPerRing);
        Gizmos.color = Color.cyan;

        Vector3 previousPoint = Vector3.zero;
        bool hasPreviousPoint = false;

        foreach (Vector3 direction in EnumerateOutlineDirections(outlineSegments))
        {
            Vector3 point = origin + direction * _controller.RayDistance;

            if (hasPreviousPoint)
            {
                Gizmos.DrawLine(previousPoint, point);
            }

            Gizmos.DrawLine(origin, point);
            previousPoint = point;
            hasPreviousPoint = true;
        }
    }

    private IEnumerable<Vector3> EnumerateOutlineDirections(int segmentCount)
    {
        Quaternion rotation = transform.rotation;

        for (int segmentIndex = 0; segmentIndex <= segmentCount; segmentIndex++)
        {
            float yaw = (360.0f / segmentCount) * segmentIndex;
            Vector3 direction = _raycastAbility.GetConeDirection(rotation, _controller.ConeAngle, yaw);
            yield return direction;
        }
    }
}
