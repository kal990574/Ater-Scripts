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

            float delay = distance / e.ExpandSpeed;
            detectable.NotifyWaveReached(delay);
        }
    }
}