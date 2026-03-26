using System;
using UnityEngine;

//아이템을 타게팅 할 수 있음
public interface IDetectableObject
{
    Transform Transform { get; }
    void OnDetectEnter();        //타게팅된경우 -> 호버 On
    void OnDetectExit();         //타겟팅이 풀린 경우 -> 호버 OFF
    
    event Action<bool> OnDetected;
}