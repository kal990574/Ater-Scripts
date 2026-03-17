using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class LidarEffectAbility : LidarAbility
{
    [Title("Reference")]
    [SerializeField] private Transform _shootTransform;
    [SerializeField] private LineRenderer _lineRenderer;
    [FormerlySerializedAs("_targetingAbility")] [SerializeField] private LidarRaycastAbility raycastAbility;

    [Title("Effect Parameter")]
    [SerializeField] private float _drawDelay = 0.5f;

    [Title("Debug Cache")]
    [SerializeField, ReadOnly] private bool _hasTarget = false;
    [SerializeField, ReadOnly] private bool _isLineVisible = false;
    [SerializeField, ReadOnly] private float _lastDrawTime = -999.0f;

    protected override void Awake()
    {
        base.Awake();

        if (_shootTransform == null && _controller != null)
        {
            _shootTransform = _controller.ShootPoint;
        }

        if (raycastAbility == null && _controller != null)
        {
            raycastAbility = _controller.GetAbility<LidarRaycastAbility>();
        }

        if (_lineRenderer != null)
        {
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.positionCount = 2;
        }

        ClearLine();
    }

    public void DrawLidarEffect(LidarScannableObject target)
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

        DrawRaycastLine();
    }

    public void ResetLine()
    {
        ClearLine();
        _hasTarget = false;
    }

    private bool CanDrawEffect()
    {
        if (_shootTransform == null)
        {
            return false;
        }

        if (_lineRenderer == null)
        {
            return false;
        }

        if (Time.time < _lastDrawTime + _drawDelay)
        {
            return false;
        }

        return true;
    }

    private void DrawTargetLine(LidarScannableObject target)
    {
        Vector3 targetPoint = GetPointOnTargetSurface(target);

        SetLineColor(Color.green);
        SetLine(_shootTransform.position, targetPoint);

        _hasTarget = true;
        _isLineVisible = true;
    }

    private void DrawRaycastLine()
    {
        if (raycastAbility == null || raycastAbility.RayResults.Count == 0)
        {
            ResetLine();
            return;
        }

        LidarRayData rayData = raycastAbility.RayResults[Random.Range(0, raycastAbility.RayResults.Count)];

        SetLineColor(Color.red);
        SetLine(_shootTransform.position, rayData.EndPoint);

        _hasTarget = false;
        _isLineVisible = true;
    }

    private Vector3 GetPointOnTargetSurface(LidarScannableObject target)
    {
        if (raycastAbility != null && raycastAbility.TryGetTargetHits(target, out List<RaycastHit> hitList) == true)
        {
            RaycastHit hit = hitList[Random.Range(0, hitList.Count)];
            return hit.point;
        }

        Collider targetCollider = target.GetComponentInChildren<Collider>();
        if (targetCollider != null)
        {
            return targetCollider.ClosestPoint(_shootTransform.position);
        }

        return target.transform.position;
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
        _isLineVisible = false;
    }

    private void SetLineColor(Color color)
    {
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
    }
}
