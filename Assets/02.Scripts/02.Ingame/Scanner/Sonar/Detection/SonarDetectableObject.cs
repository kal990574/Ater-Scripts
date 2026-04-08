using System;
using UnityEngine;

namespace _02.Scripts.Sonar
{
    [DisallowMultipleComponent]
    public class SonarDetectableObject : MonoBehaviour, ISonarDetectable
    {
        public event Action<float> OnSonarWaveReached;

        public void NotifyWaveReached(float delay)
        {
            OnSonarWaveReached?.Invoke(delay);
        }
    }
}