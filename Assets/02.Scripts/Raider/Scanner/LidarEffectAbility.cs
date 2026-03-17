using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

// 라이더 스캔의 연출을 담당
public class LidarEffectAbility : MonoBehaviour
{
    [Title("Reference")]
    [SerializeField] private Transform _shootTransform;
    [SerializeField] private LineRenderer _lineRenderer;

    [Title("Edit Parameter")]
    [SerializeField] private float _rayDistance = 3.0f;
    [SerializeField] private float _rayMaxAngle = 10.0f; // 도 단위
    [SerializeField] private float _rayShootDelay = 0.1f;

    [Title("Cache Parameter")]
    private bool _hasTarget = false;

    private void Awake()
    {
        //라인렌더러 초기 설정
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.positionCount = 2;
    }

    //이벤트 구독방식이 좋을까?
    public void DrawTargetLine(List<RaycastHit> targetHit)
    {
        int randomIndex = Random.Range(0, targetHit.Count);
        RaycastHit selectedHit = targetHit[randomIndex];

        DrawLineToPoint(selectedHit.point, Color.green);
    }
    
    private void DrawRandomLine()
    {
        float yaw = Random.Range(-_rayMaxAngle, _rayMaxAngle);
        float pitch = Random.Range(-_rayMaxAngle, _rayMaxAngle);

        Quaternion rot =
            Quaternion.AngleAxis(yaw, _shootTransform.up) *
            Quaternion.AngleAxis(pitch, _shootTransform.right);

        Vector3 direction = rot * _shootTransform.forward;
        Vector3 endPoint = _shootTransform.position + direction * _rayDistance;

        DrawLineToPoint(endPoint, Color.red);
    }

    private void DrawLineToPoint(Vector3 endPoint, Color lineColor)
    {
        _lineRenderer.startColor = lineColor;
        _lineRenderer.endColor = lineColor;

        Vector3 startPoint = _shootTransform.position;

        _lineRenderer.SetPosition(0, startPoint);
        _lineRenderer.SetPosition(1, endPoint);
    }
}