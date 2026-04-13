using DG.Tweening;
using UnityEngine;

namespace _02.Scripts.Sonar
{
    public class SonarHighlightShaderModifier : MonoBehaviour
    {
        private const string OUTLINE_COLOR_NAME = "_OutlineColor";
        private const string OUTLINE_THICKNESS_NAME = "_OutlineThickness";
        private const string HIT_BLEND_NAME = "_HitBlend";

        [Header("Required References")]
        [SerializeField] private AllInOneShaderController _shaderController;
        [SerializeField] private SonarHighlightConfigSO _config;

        private SonarDetectableObject _detectable;
        private InteractTargetShaderModifier _lidarModifier;
        private Sequence _activeSequence;

        private float _currentHitBlend;
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
                _shaderController = GetComponentInChildren<AllInOneShaderController>();
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

            // 아웃라인 활성화 + HitBlend 플래시
            seq.AppendCallback(() =>
            {
                IsHighlighting = true;
                _currentOutlineThickness = _config.OutlineThickness;
                SetOutlineThickness(_config.OutlineThickness);
                SetOutlineColor(_config.SonarOutlineColor);
                _shaderController.SetOutlineType(AllInOneShaderController.OutlineType.Simple);
            });
            seq.Append(CreateHitBlendTween(_config.HitBlendPeak, _config.HitBlendAttackDuration)
                .SetEase(_config.HitBlendAttackEase));
            seq.Append(CreateHitBlendTween(0f, _config.HitBlendAttackDuration * 2f)
                .SetEase(Ease.InQuad));

            // 아웃라인 유지
            seq.AppendInterval(_config.HighlightDuration);

            // 아웃라인 두께 페이드아웃
            seq.Append(CreateOutlineThicknessFadeTween(0f, _config.FadeOutDuration)
                .SetEase(Ease.InQuad));

            // 프로퍼티 리셋 + 라이다 상태 복원
            seq.AppendCallback(() =>
            {
                IsHighlighting = false;
                _shaderController.SetOutlineType(AllInOneShaderController.OutlineType.None);
                SetOutlineThickness(0f);
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
            _currentOutlineThickness = 0f;
            SetHitBlend(0f);
            SetOutlineThickness(0f);
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

        private Tween CreateOutlineThicknessFadeTween(float target, float duration)
        {
            return DOTween.To(
                () => _currentOutlineThickness,
                value =>
                {
                    _currentOutlineThickness = value;
                    SetOutlineThickness(value);
                },
                target,
                duration);
        }

        #endregion

        #region Shader Property Helpers

        private void SetOutlineColor(Color value)
        {
            _shaderController.SetColor(OUTLINE_COLOR_NAME, value);
        }

        private void SetHitBlend(float value)
        {
            _shaderController.SetFloat(HIT_BLEND_NAME, value);
        }

        private void SetOutlineThickness(float value)
        {
            _shaderController.SetFloat(OUTLINE_THICKNESS_NAME, value);
        }

        #endregion
    }
}