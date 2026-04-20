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
    private LidarSurfaceHitSample _currentTargetHitSample;
    private bool _hasCurrentTargetHitSample;
    private float _lastDrawTime = -999.0f;
    
    public LidarEffect(LidarScanFeature scanFeature)
    {
        _scanFeature = scanFeature;
        _muzzle = scanFeature.Muzzle;
        _lineRenderer = scanFeature.LineRenderer;
        _config = scanFeature.Config;
        
        ClearLine();
    }

    public void DrawLidarEffect(
        IReadOnlyList<LidarRayData> rayDatas,
        ScannableObject target)
    {
        _hasCurrentTargetHitSample = false;

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
        _hasCurrentTargetHitSample = false;
        ClearLine();
    }

    public bool TryGetCurrentTargetHitSample(out LidarSurfaceHitSample hitSample)
    {
        hitSample = _currentTargetHitSample;
        return _hasCurrentTargetHitSample;
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

    private void DrawTargetLine(ScannableObject target)
    {
        if (!TryGetRandomTargetSurfaceSample(target, out LidarSurfaceHitSample hitSample))
        {
            ResetLine();
            return;
        }

        _currentTargetHitSample = hitSample;
        _hasCurrentTargetHitSample = true;
        SetLineColor(_config.OnTargetGradient);
        SetLine(_muzzle.position, hitSample.Point);
    }

    private void DrawRaycastLine(IReadOnlyList<LidarRayData> rayDatas)
    {
        if (rayDatas == null || rayDatas.Count == 0)
        {
            ResetLine();
            return;
        }

        LidarRayData rayData = rayDatas[Random.Range(0, rayDatas.Count)];
        SetLineColor(_config.NonTargetGradient);
        SetLine(_muzzle.position, rayData.EndPoint);
    }

    private bool TryGetRandomTargetSurfaceSample(
        ScannableObject target,
        out LidarSurfaceHitSample hitSample)
    {
        hitSample = default;

        if (target == null)
        {
            return false;
        }

        _colliders.Clear();
        target.GetComponentsInChildren(_colliders);
        if (_colliders.Count == 0)
        {
            return false;
        }

        Collider targetCollider = _colliders[Random.Range(0, _colliders.Count)];
        if (!TrySampleRandomSurfacePoint(targetCollider, out Vector3 sampledPoint, out Vector3 sampledNormal))
        {
            sampledPoint = targetCollider.ClosestPoint(_muzzle.position);
            sampledNormal = ResolveFallbackNormal(targetCollider, sampledPoint);
        }

        hitSample = new LidarSurfaceHitSample(target, sampledPoint, sampledNormal, Vector3.Distance(_muzzle.position, sampledPoint));
        return true;
    }

    private bool TrySampleRandomSurfacePoint(Collider targetCollider, out Vector3 sampledPoint, out Vector3 sampledNormal)
    {
        sampledPoint = default;
        sampledNormal = Vector3.up;

        if (targetCollider == null)
        {
            return false;
        }

        Bounds bounds = targetCollider.bounds;
        Vector3 center = bounds.center;
        float radius = bounds.extents.magnitude;
        if (radius <= Mathf.Epsilon)
        {
            sampledPoint = center;
            sampledNormal = Vector3.up;
            return false;
        }

        for (int attempt = 0; attempt < 8; attempt++)
        {
            Vector3 outwardDirection = Random.onUnitSphere;
            Vector3 sampleOrigin = center + outwardDirection * radius * 2.0f;
            Vector3 surfacePoint = bounds.ClosestPoint(sampleOrigin);
            if ((surfacePoint - sampleOrigin).sqrMagnitude <= 0.0001f)
            {
                continue;
            }

            sampledPoint = surfacePoint;
            sampledNormal = outwardDirection;
            return true;
        }

        sampledPoint = targetCollider.ClosestPoint(_muzzle.position);
        sampledNormal = ResolveFallbackNormal(targetCollider, sampledPoint);
        return false;
    }

    private Vector3 ResolveFallbackNormal(Collider targetCollider, Vector3 sampledPoint)
    {
        if (targetCollider == null)
        {
            return Vector3.up;
        }

        Vector3 normal = sampledPoint - targetCollider.bounds.center;
        if (normal.sqrMagnitude <= 0.0001f)
        {
            return Vector3.up;
        }

        return normal.normalized;
    }

    private void SetLine(Vector3 startPoint, Vector3 endPoint)
    {
        if (_lineRenderer == null)
        {
            return;
        }

        endPoint = GetClampedEndPoint(startPoint, endPoint);

        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, startPoint);
        _lineRenderer.SetPosition(1, endPoint);
    }

    private Vector3 GetClampedEndPoint(Vector3 startPoint, Vector3 endPoint)
    {
        float maxLineLength = _config.MaxLineLength;

        if (maxLineLength <= 0.0f)
        {
            return endPoint;
        }

        Vector3 lineVector = endPoint - startPoint;
        float lineLength = lineVector.magnitude;

        if (lineLength <= maxLineLength || lineLength <= Mathf.Epsilon)
        {
            return endPoint;
        }

        return startPoint + lineVector / lineLength * maxLineLength;
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

    private void SetLineColor(Gradient color)
    {
        if (_lineRenderer == null)
        {
            return;
        }

        _lineRenderer.colorGradient = color;
        _lineRenderer.colorGradient = color;
    }
}
