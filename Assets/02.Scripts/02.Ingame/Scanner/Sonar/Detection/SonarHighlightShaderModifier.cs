using DG.Tweening;
using UnityEngine;

namespace _02.Scripts.Sonar
{
    public class SonarHighlightShaderModifier : MonoBehaviour
    {
        private const string HIT_BLEND_NAME = "_HitBlend";
        private const string HIT_COLOR_NAME = "_HitColor";
        private const string HIT_GLOW_NAME = "_HitGlow";

        [Header("Required References")]
        [SerializeField] private AllInOneShaderController _shaderController;
        [SerializeField] private SonarHighlightConfigSO _config;

        private SonarDetectableObject _detectable;
        private InteractTargetShaderModifier _lidarModifier;
        private Sequence _activeSequence;

        private float _currentHitBlend;

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

            if (_lidarModifier == null)
            {
                _lidarModifier = GetComponent<InteractTargetShaderModifier>();
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

        #region Sonar Highlight Handler

        private void OnSonarWaveReached(float delay)
        {
            KillActiveSequence();
            ResetState();

            var seq = DOTween.Sequence();

            // 소나 파동 도달 대기
            seq.AppendInterval(delay);

            // HitBlend 색상 세팅
            seq.AppendCallback(() =>
            {
                IsHighlighting = true;
                _shaderController.SetColor(HIT_COLOR_NAME, _config.HitColor);
                _shaderController.SetFloat(HIT_GLOW_NAME, _config.HitGlow);
            });

            // Attack: 0 → Peak
            seq.Append(CreateHitBlendTween(_config.HitBlendPeak, _config.HitBlendAttackDuration)
                .SetEase(_config.HitBlendAttackEase));

            // Decay: Peak → 0
            seq.Append(CreateHitBlendTween(0f, _config.HitBlendAttackDuration * 2f)
                .SetEase(Ease.InQuad));

            // Cleanup
            seq.AppendCallback(() =>
            {
                IsHighlighting = false;
                SetHitBlend(0f);

                if (_lidarModifier != null)
                {
                    _lidarModifier.SynchronizeCurrentState();
                }
            });

            seq.SetLink(gameObject, LinkBehaviour.KillOnDisable);
            seq.OnKill(() => _activeSequence = null);
            _activeSequence = seq;
        }

        private void ResetState()
        {
            IsHighlighting = false;
            _currentHitBlend = 0f;
            SetHitBlend(0f);
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

        private Tween CreateHitBlendTween(float target, float duration)
        {
            return DOTween.To(
                () => _currentHitBlend,
                value =>
                {
                    _currentHitBlend = value;
                    SetHitBlend(value);
                },
                target,
                duration);
        }

        #endregion

        #region Shader Property Helpers

        private void SetHitBlend(float value)
        {
            _shaderController.SetFloat(HIT_BLEND_NAME, value);
        }

        #endregion
    }
}