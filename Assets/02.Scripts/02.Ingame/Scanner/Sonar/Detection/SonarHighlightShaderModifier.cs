using DG.Tweening;
using UnityEngine;

namespace _02.Scripts.Sonar
{
    public class SonarHighlightShaderModifier : MonoBehaviour
    {
        private const string OUTLINE_COLOR_NAME = "_OutlineColor";
        private const string HIT_BLEND_NAME = "_HitBlend";

        [Header("Required References")]
        [SerializeField] private AllInOneShaderController _shaderController;
        [SerializeField] private SonarHighlightConfigSO _config;

        private SonarDetectableObject _detectable;
        private Sequence _activeSequence;

        private float _currentHitBlend;
        private float _currentOutlineAlpha;

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
                _currentOutlineAlpha = 1f;
                SetOutlineColor(_config.SonarOutlineColor);
                _shaderController.SetOutlineType(AllInOneShaderController.OutlineType.Simple);
            });
            seq.Append(CreateHitBlendTween(_config.HitBlendPeak, _config.HitBlendAttackDuration)
                .SetEase(_config.HitBlendAttackEase));
            seq.Append(CreateHitBlendTween(0f, _config.HitBlendAttackDuration * 2f)
                .SetEase(Ease.InQuad));

            // 아웃라인 유지
            seq.AppendInterval(_config.HighlightDuration);

            // 아웃라인 페이드아웃
            seq.Append(CreateOutlineAlphaFadeTween(0f, _config.FadeOutDuration)
                .SetEase(Ease.InQuad));

            // 프로퍼티 리셋
            seq.AppendCallback(() =>
            {
                _shaderController.SetOutlineType(AllInOneShaderController.OutlineType.None);
                SetHitBlend(0f);
            });

            seq.SetLink(gameObject, LinkBehaviour.KillOnDisable);
            seq.OnKill(() => _activeSequence = null);
            _activeSequence = seq;
        }

        private void ResetState()
        {
            _currentHitBlend = 0f;
            _currentOutlineAlpha = 0f;
            SetHitBlend(0f);
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

        private Tween CreateOutlineAlphaFadeTween(float target, float duration)
        {
            return DOTween.To(
                () => _currentOutlineAlpha,
                value =>
                {
                    _currentOutlineAlpha = value;
                    Color color = _config.SonarOutlineColor;
                    color.a = value;
                    SetOutlineColor(color);
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

        #endregion
    }
}