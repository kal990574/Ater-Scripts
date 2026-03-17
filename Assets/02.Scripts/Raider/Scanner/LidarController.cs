using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

//실질적인 라이더 스캐닝 관리, 모든 하위 컴포넌트 관리
public class LidarController : MonoBehaviour
{
    //라이더의 스텟관리(발사거리, 발사각도 등)
    [Title("Reference")]
    [SerializeField] private Transform _originPos; //발사 위치
    
    [Title("Laider Settings")]
    [SerializeField] private float _rayDistance = 10.0f;
    [SerializeField] private float _coneAngle = 45.0f;
    [SerializeField] private int _ringCount = 4;
    [SerializeField] private int _raysPerRing = 12;
    [SerializeField] private Vector3 _originOffset = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] private LayerMask _hitMask = ~0;
    
    public float RayDistance => _rayDistance;
    public float ConeAngle => _coneAngle;
    public int RingCount => _ringCount;
    public int RaysPerRing => _raysPerRing;
    public LayerMask HitMask => _hitMask;
    public Vector3 StartPos => _originPos.position  + _originOffset;

    [Title("Caching")] private List<LidarAbility> _abilities;
}
