using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LidarEffect
{
    private readonly LidarScanFeature _scanFeature;
    private readonly Transform _muzzle;
    private readonly LineRenderer _lineRenderer;
    private readonly LidarScanConfigSO _config;
    
    private readonly List<Collider> _colliders = new();
    private float _lastDrawTime = -999.0f;
    
    public LidarEffect(LidarScanFeature scanFeature)
    {
        _scanFeature = scanFeature;
        _muzzle = scanFeature.Muzzle;
        _lineRenderer = scanFeature.LineRenderer;
        _config = scanFeature.Config;
        
        ClearLine();
    }

    public void DrawLidarEffect(IReadOnlyList<LidarRayData> rayDatas, InteractScanObject target)
    {
        if (CanDrawEffect() == false)
        {
            return;
        }

        _lastDrawTime = Time.time;

        if (target != null)
        {
            DrawTargetLine(target);
            return;
        }

        DrawRaycastLine(rayDatas);
    }

    public void ResetLine()
    {
        ClearLine();
    }

    private bool CanDrawEffect()
    {
        if (_muzzle == null || _lineRenderer == null)
        {
            return false;
        }

        if (Time.time < _lastDrawTime + _config.DrawDelay)
        {
            return false;
        }

        return true;
    }

    private void DrawTargetLine(InteractScanObject target)
    {
        Vector3 targetPoint = GetPointOnTargetSurface(target);
        SetLineColor(Color.green);
        SetLine(_muzzle.position, targetPoint);
    }

    private void DrawRaycastLine(IReadOnlyList<LidarRayData> rayDatas)
    {
        if (rayDatas == null || rayDatas.Count == 0)
        {
            ResetLine();
            return;
        }

        LidarRayData rayData = rayDatas[Random.Range(0, rayDatas.Count)];
        SetLineColor(Color.red);
        SetLine(_muzzle.position, rayData.EndPoint);
        Debug.Log(rayData.EndPoint.ToString());
    }

    private Vector3 GetPointOnTargetSurface(InteractScanObject target)
    {
        if (target == null)
        {
            return _muzzle != null ? _muzzle.position : Vector3.zero;
        }

        _colliders.Clear();
        target.GetComponentsInChildren(_colliders);

        if (_colliders.Count > 0)
        {
            Collider targetCollider = _colliders[Random.Range(0, _colliders.Count)];
            Vector3 sampledPoint = SampleRandomSurfacePoint(targetCollider);

            if (sampledPoint != targetCollider.bounds.center)
            {
                return sampledPoint;
            }

            return targetCollider.ClosestPoint(_muzzle.position);
        }

        return target.transform.position;
    }

    private Vector3 SampleRandomSurfacePoint(Collider targetCollider)
    {
        Bounds bounds = targetCollider.bounds;
        Vector3 center = bounds.center;
        float radius = bounds.extents.magnitude;

        if (radius <= Mathf.Epsilon)
        {
            return center;
        }

        for (int attempt = 0; attempt < 8; attempt++)
        {
            Vector3 direction = Random.onUnitSphere;
            Vector3 sampleOrigin = center + direction * radius * 2.0f;
            Vector3 surfacePoint = targetCollider.ClosestPoint(sampleOrigin);

            if ((surfacePoint - sampleOrigin).sqrMagnitude > 0.0001f)
            {
                return surfacePoint;
            }
        }

        return center;
    }

    private void SetLine(Vector3 startPoint, Vector3 endPoint)
    {
        if (_lineRenderer == null)
        {
            return;
        }

        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, startPoint);
        _lineRenderer.SetPosition(1, endPoint);
    }

    private void ClearLine()
    {
        if (_lineRenderer == null)
        {
            return;
        }

        _lineRenderer.enabled = false;

        if (_muzzle == null)
        {
            return;
        }

        Vector3 origin = _muzzle.position;
        _lineRenderer.SetPosition(0, origin);
        _lineRenderer.SetPosition(1, origin);
    }

    private void SetLineColor(Color color)
    {
        if (_lineRenderer == null)
        {
            return;
        }

        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
    }
}
