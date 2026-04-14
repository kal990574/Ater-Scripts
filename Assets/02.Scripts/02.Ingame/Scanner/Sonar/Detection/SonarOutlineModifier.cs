using DG.Tweening;
using UnityEngine;

namespace _02.Scripts.Sonar
{
    public class SonarOutlineModifier : MonoBehaviour
    {
        private const string OUTLINE_COLOR_NAME = "_OutlineColor";
        private const string OUTLINE_THICKNESS_NAME = "_OutlineThickness";

        [Header("Required References")]
        [SerializeField] private AllInOneShaderController _shaderController;
        [SerializeField] private SonarOutlineConfigSO _config;

        private SonarDetectableObject _detectable;
        private Sequence _activeSequence;
        private float _currentOutlineThickness;

        public bool IsHighlighting { get; private set; }

        #region Lifecycle

        private void Start()
        {
            CacheReferences();
            InitShaderController();
            InitConfig();
            SubscribeEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
            KillActiveSequence();
        }

        #endregion

        #region Initialization

        private void CacheReferences()
        {
            if (_shaderController == null)
            {
                _shaderController = GetComponent<AllInOneShaderController>();
            }

            if (_detectable == null)
            {
                _detectable = GetComponentInParent<SonarDetectableObject>();
            }
        }

        private void InitShaderController()
        {
            if (_shaderController == null)
            {
                enabled = false;
                return;
            }

            _shaderController.Init();
        }

        private void InitConfig()
        {
            if (_config == null)
            {
                enabled = false;
            }
        }

        #endregion

        #region Event Subscription

        private void SubscribeEvents()
        {
            if (_detectable != null)
            {
                _detectable.OnSonarWaveReached += OnSonarWaveReached;
            }
        }

        private void UnsubscribeEvents()
        {
            if (_detectable != null)
            {
                _detectable.OnSonarWaveReached -= OnSonarWaveReached;
            }
        }

        #endregion

        #region Sonar Outline Handler

        private void OnSonarWaveReached(float delay)
        {
            KillActiveSequence();
            ResetState();

            var seq = DOTween.Sequence();

            // 소나 파동 도달 대기
            seq.AppendInterval(delay);

            // 아웃라인 즉시 활성화
            seq.AppendCallback(() =>
            {
                IsHighlighting = true;
                _currentOutlineThickness = _config.OutlineThickness;
                _shaderController.SetColor(OUTLINE_COLOR_NAME, _config.OutlineColor);
                _shaderController.SetOutlineType(AllInOneShaderController.OutlineType.Simple);
                _shaderController.SetFloat(OUTLINE_THICKNESS_NAME, _config.OutlineThickness);
            });

            // 두께 점진적 감소 → 0
            seq.Append(CreateOutlineThicknessTween(0f, _config.OutlineDuration)
                .SetEase(_config.OutlineDecayEase));

            // Cleanup
            seq.AppendCallback(() =>
            {
                IsHighlighting = false;
                _shaderController.SetOutlineType(AllInOneShaderController.OutlineType.None);
                _shaderController.SetFloat(OUTLINE_THICKNESS_NAME, 0f);
            });

            seq.SetLink(gameObject, LinkBehaviour.KillOnDisable);
            seq.OnKill(() => _activeSequence = null);
            _activeSequence = seq;
        }

        private void ResetState()
        {
            IsHighlighting = false;
            _currentOutlineThickness = 0f;
            _shaderController.SetFloat(OUTLINE_THICKNESS_NAME, 0f);
            _shaderController.SetOutlineType(AllInOneShaderController.OutlineType.None);
        }

        private void KillActiveSequence()
        {
            if (_activeSequence == null)
            {
                return;
            }

            if (_activeSequence.IsActive())
            {
                _activeSequence.Kill();
            }

            _activeSequence = null;
        }

        #endregion

        #region Tween Factory

        private Tween CreateOutlineThicknessTween(float target, float duration)
        {
            return DOTween.To(
                () => _currentOutlineThickness,
                value =>
                {
                    _currentOutlineThickness = value;
                    _shaderController.SetFloat(OUTLINE_THICKNESS_NAME, value);
                },
                target,
                duration);
        }

        #endregion
    }
}