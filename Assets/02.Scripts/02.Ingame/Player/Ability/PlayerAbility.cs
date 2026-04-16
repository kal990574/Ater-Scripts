using System;
using UnityEngine;

namespace _02.Scripts.Player
{
    public class PlayerAbility : MonoBehaviour
    {
        protected PlayerController _owner;

        protected virtual void Awake()
        {
            _owner = GetComponentInParent<PlayerController>();
        }
    }
}
