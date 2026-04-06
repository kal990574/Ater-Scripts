using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _02.Scripts.AIHint.Component
{
    public class PhoneViewController : MonoBehaviour
    {
        [Header("폰 모델")]
        [SerializeField] private GameObject _phoneModel;

        [Header("연출")]
        [SerializeField] private float _animDuration = 0.3f;
        [SerializeField] private Vector3 _showPosition = new(0f, -0.3f, 0.5f);
        [SerializeField] private Vector3 _hideOffset = new(0f, -0.5f, 0f);

        private Vector3 _hidePosition;

        private void Awake()
        {
            _hidePosition = _showPosition + _hideOffset;
            _phoneModel.transform.localPosition = _hidePosition;
            _phoneModel.SetActive(false);
        }

        public async UniTask Show()
        {
            _phoneModel.SetActive(true);
            await AnimatePosition(_hidePosition, _showPosition);
        }

        public async UniTask Hide()
        {
            Vector3 current = _phoneModel.transform.localPosition;
            await AnimatePosition(current, _hidePosition);
            _phoneModel.SetActive(false);
        }

        private async UniTask AnimatePosition(Vector3 from, Vector3 to)
        {
            float elapsed = 0f;
            while (elapsed < _animDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / _animDuration);
                _phoneModel.transform.localPosition = Vector3.Lerp(from, to, t);
                await UniTask.Yield();
            }

            _phoneModel.transform.localPosition = to;
        }
    }
}