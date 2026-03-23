using UnityEngine;

//현재 수행되는 플레이어와 상호작용의 모든 것
public class HoverableObject : IHoverable
{
    [SerializeField] private Transform HoverTarget;
}
