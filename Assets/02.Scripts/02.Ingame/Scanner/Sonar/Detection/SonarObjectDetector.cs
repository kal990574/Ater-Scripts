using System;
using System.Collections.Generic;
using UnityEngine;
using _02.Scripts.Sonar;

public class SonarObjectDetector : MonoBehaviour
{
    [SerializeField] private int _maxDetections = 32;
    [SerializeField] private LayerMask _detectionLayer = ~0;

    private Collider[] _hitBuffer;
    private readonly HashSet<SonarDetectableObject> _processedThisFrame = new();
    private IDisposable _subscription;

    private void Start()
    {
        _hitBuffer = new Collider[_maxDetections];

        var hub = GameEventHub.Instance;
        if (hub != null)
        {
            _subscription = hub.Subscribe<SonarScanStartedRawEvent>(OnSonarScan);
        }
    }

    private void OnDestroy()
    {
        _subscription?.Dispose();
    }

    private void OnSonarScan(SonarScanStartedRawEvent e)
    {
        int hitCount = Physics.OverlapSphereNonAlloc(e.Origin, e.ScanRadius, _hitBuffer, _detectionLayer);

        float cosHalfAngle = Mathf.Cos(e.ScanAngle * 0.5f * Mathf.Deg2Rad);
        Vector3 dirNormalized = e.Direction.normalized;
        float expandDuration = e.ScanRadius / e.ExpandSpeed;

        _processedThisFrame.Clear();

        for (int i = 0; i < hitCount; i++)
        {
            var detectable = _hitBuffer[i].GetComponentInParent<SonarDetectableObject>();
            if (detectable == null) continue;
            if (!_processedThisFrame.Add(detectable)) continue;

            Vector3 toTarget = detectable.transform.position - e.Origin;
            float distance = toTarget.magnitude;
            if (distance > e.ScanRadius) continue;

            if (distance > 0.01f)
            {
                float dot = Vector3.Dot(dirNormalized, toTarget / distance);
                if (dot < cosHalfAngle) continue;
            }

            float delay = GetDelayFromCurve(e.ExpandCurve, distance, e.ScanRadius, expandDuration);
            detectable.NotifyWaveReached(delay);
        }
    }

    private static float GetDelayFromCurve(AnimationCurve curve, float distance, float maxRadius, float expandDuration)
    {
        if (curve == null)
        {
            return distance / maxRadius * expandDuration;
        }

        float normalizedDist = distance / maxRadius;
        float lo = 0f;
        float hi = 1f;

        for (int i = 0; i < 16; i++)
        {
            float mid = (lo + hi) * 0.5f;
            if (curve.Evaluate(mid) < normalizedDist)
                lo = mid;
            else
                hi = mid;
        }

        return (lo + hi) * 0.5f * expandDuration;
    }
}
