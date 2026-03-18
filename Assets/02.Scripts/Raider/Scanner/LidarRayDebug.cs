using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LidarRayDebug : MonoBehaviour
{
    [SerializeField] private LidarController _controller;
    [SerializeField] private LidarRaycastAbility _raycastAbility;

    [SerializeField] private bool _activateDebug = false;
    [SerializeField] private float _pointRadius = 0.05f;

    private void OnDrawGizmos()
    {
        if (CanDraw() == false || !_activateDebug)
        {
            return;
        }
        
        Vector3 origin = _controller.StartPos;
        if (!Application.isPlaying || !_controller.IsOnScan)
        {
            _raycastAbility.Scan();
        }

        DrawRayResultsInPlayMode(origin);
        DrawConeOutline(origin);
    }

    

    private bool CanDraw()
    {
        if (_controller == null)
        {
            return false;
        }

        if (_raycastAbility == null)
        {
            return false;
        }

        return true;
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
    
    private void DrawRayResultsInEditMode(Vector3 origin)
    {
        foreach (var dir in _raycastAbility.EnumerateRayDirections())
        {
            DrawSingleDebugRay(origin, dir);
        }
    }

    private void DrawSingleRayResult(Vector3 origin, LidarRayData rayData)
    {
        if (rayData.IsHit == true)
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
            Gizmos.color = Color.green;
            Gizmos.DrawLine(origin, hit.point);
            Gizmos.DrawSphere(hit.point, _pointRadius);
            return;
        }

        Vector3 endPoint = origin + direction * _controller.RayDistance;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, endPoint);
        Gizmos.DrawSphere(endPoint, _pointRadius);
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

            if (hasPreviousPoint == true)
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