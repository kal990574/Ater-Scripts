using System;
using UnityEngine;
using UnityEngine.Events;

//플레이어가 감지 가능한 오브젝트에 대해 아웃라인 기능을 제공한다.
//플레이어는 해당 인터페이스만을 찾아서 타겟으로 삼는다.
public class DetectableObject : MonoBehaviour, IDetectableObject
{
    public Transform Transform => transform;
    public event Action<bool> OnDetected;
    
    [Header("Scene Event")]
    public UnityEvent DetectOnEvent;
    public UnityEvent DetectOffEvent;
    
    [ContextMenu("hover")]
    public void OnDetectEnter()
    {
        Debug.Log("Hover");
        OnDetected?.Invoke(true);
        DetectOnEvent?.Invoke();
    }

    [ContextMenu("unhover")]
    public void OnDetectExit()
    {
        OnDetected?.Invoke(false);
        DetectOffEvent?.Invoke();
    }
}
