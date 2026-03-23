using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LidarEffect : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private LidarScanFeature _scanFeature;
    [SerializeField] private Transform _shootTransform;
    [SerializeField] private LineRenderer _lineRenderer;
    

    [Header("Settings")]
    [Min(0f)]
    [SerializeField] private float _drawDelay = 0.5f;

    [Header("Debug")]
    [SerializeField] private float _lastDrawTime = -999.0f;

    private readonly List<Collider> _colliders = new();

    public void Init(LidarScanFeature scanFeature)
    {
        _scanFeature = scanFeature;
        
        if (_shootTransform == null && _scanFeature != null)
        {
            _shootTransform = _scanFeature.ShootPoint;
        }

        if (_lineRenderer != null)
        {
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.positionCount = 2;
        }

        ClearLine();
    }

    public void DrawLidarEffect(IReadOnlyList<LidarRayData> rayDatas, LidarTarget target)
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
        if (_shootTransform == null || _lineRenderer == null)
        {
            return false;
        }

        if (Time.time < _lastDrawTime + _drawDelay)
        {
            return false;
        }

        return true;
    }

    private void DrawTargetLine(LidarTarget target)
    {
        Vector3 targetPoint = GetPointOnTargetSurface(target);
        SetLineColor(Color.green);
        SetLine(_shootTransform.position, targetPoint);
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
        SetLine(_shootTransform.position, rayData.EndPoint);
    }

    private Vector3 GetPointOnTargetSurface(LidarTarget target)
    {
        if (target == null)
        {
            return _shootTransform != null ? _shootTransform.position : Vector3.zero;
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

            return targetCollider.ClosestPoint(_shootTransform.position);
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
        _lineRenderer.SetPosition(0, startPoint);
        _lineRenderer.SetPosition(1, endPoint);
    }

    private void ClearLine()
    {
        if (_lineRenderer == null || _shootTransform == null)
        {
            return;
        }

        Vector3 origin = _shootTransform.position;
        SetLine(origin, origin);
    }

    private void SetLineColor(Color color)
    {
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
    }
}
