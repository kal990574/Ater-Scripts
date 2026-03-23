using UnityEngine;
using System.Collections;

namespace _02.Scripts.Sonar
{
    public class SonarCameraFeedback : MonoBehaviour
    {
        [SerializeField] private float _shakeIntensity = 0.05f;
        [SerializeField] private float _shakeDuration = 0.15f;
        
        private Coroutine _shakeCoroutine;

        public void Play()
        {
            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
                transform.localPosition = Vector3.zero;
            }

            _shakeCoroutine = StartCoroutine(ShakeCoroutine());
        }

        private IEnumerator ShakeCoroutine()
        {
            float elapsed = 0f;

            while (elapsed < _shakeDuration)
            {
                elapsed += Time.deltaTime;
                float t = 1f - (elapsed / _shakeDuration);
                Vector3 offset = Random.insideUnitSphere * (_shakeIntensity * t);
                transform.localPosition += offset;
                yield return null;
            }
            
            transform.localPosition = Vector3.zero;
            _shakeCoroutine = null;
        }
    }
}