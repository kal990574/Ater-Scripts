using System.Collections;
using _02.Scripts.Core;
using _02.Scripts.Core.Domain;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace _02.Scripts._02.Ingame.EndingSequence
{
    public class EndingSequenceController : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField, Tooltip("씬의 PlayerController 참조")]
        private PlayerController _playerController;

        [Header("Letterbox")]
        [SerializeField, Tooltip("레터박스 상단 바 RectTransform")]
        private RectTransform _topBar;

        [SerializeField, Tooltip("레터박스 하단 바 RectTransform")]
        private RectTransform _bottomBar;

        [SerializeField, Tooltip("레터박스 바의 최대 높이 (px)")]
        private float _letterboxHeight = 120f;

        [SerializeField, Tooltip("레터박스 닫히는 시간 (초)")]
        private float _letterboxInDuration = 0.8f;

        [SerializeField, Tooltip("레터박스 열리는 시간 (초)")]
        private float _letterboxOutDuration = 0.4f;

        [SerializeField, Tooltip("HUD CanvasGroup (레터박스 진입 시 페이드아웃)")]
        private CanvasGroup _hudCanvasGroup;

        [Header("Glitch")]
        [SerializeField, Tooltip("글리치 오버레이 UI 컴포넌트")]
        private GlitchOverlayUI _glitchOverlay;

        [Header("Text UI")]
        [SerializeField, Tooltip("텍스트 패널 CanvasGroup (알파 제어용)")]
        private CanvasGroup _textCanvasGroup;

        [SerializeField, Tooltip("엔딩 로그 텍스트 (타이핑 표시)")]
        private TMP_Text _logText;

        [Header("Mannequin")]
        [SerializeField, Tooltip("점프스케어 마네킹 오브젝트 (비활성 상태로 배치)")]
        private GameObject _mannequin;

        [Header("Blackout")]
        [SerializeField, Tooltip("암전용 CanvasGroup (alpha 0→1)")]
        private CanvasGroup _blackoutCanvasGroup;

        [Header("Timing")]
        [SerializeField, Tooltip("씬 진입 후 자유 탐색 시간 (초)")]
        private float _freeRoamDuration = 7f;

        [SerializeField, Tooltip("타이핑 글자당 간격 (초)")]
        private float _typingSpeed = 0.04f;

        [SerializeField, Tooltip("줄 간 대기 시간 (초)")]
        private float _linePause = 1.0f;

        [SerializeField, Tooltip("빈 줄 대기 시간 비율 (_linePause에 곱해짐)")]
        private float _emptyLinePauseRatio = 0.5f;

        [SerializeField, Tooltip("특수문자(…, —) 딜레이 배수")]
        private float _specialCharDelayMultiplier = 3f;

        [SerializeField, Tooltip("마침표(.) 딜레이 배수")]
        private float _periodDelayMultiplier = 2f;

        [SerializeField, Tooltip("레터박스 아웃 후 마네킹 등장까지 정적 시간 (초)")]
        private float _preJumpScareDelay = 2f;

        [SerializeField, Tooltip("마네킹 노출 시간 (초)")]
        private float _mannequinExposure = 0.15f;

        [SerializeField, Tooltip("암전 후 엔딩 이벤트까지 대기 시간 (초)")]
        private float _blackoutDuration = 1.5f;

        [Header("Glitch Intensity")]
        [SerializeField, Tooltip("글리치 시작 강도 (0~1)")]
        private float _glitchIntensityStart = 0.3f;

        [SerializeField, Tooltip("글리치 최대 강도 (0~1)")]
        private float _glitchIntensityEnd = 1f;

        [Header("Audio")]
        [SerializeField, Tooltip("글리치 루프 사운드 키")]
        private SoundKeyReference _glitchSfxKey;

        [SerializeField, Tooltip("점프스케어 충격음 키")]
        private SoundKeyReference _jumpScareSfxKey;

        [SerializeField, Tooltip("타이핑 효과음 키")]
        private SoundKeyReference _typingSfxKey;

        [SerializeField, Tooltip("타이핑 사운드 재생 간격 (N글자마다 1회)")]
        private int _typingSoundInterval = 2;

        [Header("Events")]
        [SerializeField, Tooltip("엔딩 시퀀스 완료 시 호출 (크레딧 로드 등)")]
        private UnityEvent _onSequenceEnd;

        private Sequence _seq;
        private Animator _hudAnimator;

        private void Start()
        {
            Play();
        }

        [ContextMenu("Play")]
        public void Play()
        {
            StartCoroutine(EndingSequence());
        }

        private IEnumerator EndingSequence()
        {
            yield return new WaitForSeconds(_freeRoamDuration);

            _playerController.EnterCutsceneMode();
            Managers.Get<IGameManager>().EnterTransition();
            _hudAnimator = _hudCanvasGroup.GetComponent<Animator>();
            if (_hudAnimator != null) _hudAnimator.enabled = false;

            _seq = DOTween.Sequence().SetUpdate(true);
            _seq.Append(_topBar
                .DOSizeDelta(new Vector2(_topBar.sizeDelta.x, _letterboxHeight), _letterboxInDuration)
                .SetEase(Ease.OutQuad));
            _seq.Join(_bottomBar
                .DOSizeDelta(new Vector2(_bottomBar.sizeDelta.x, _letterboxHeight), _letterboxInDuration)
                .SetEase(Ease.OutQuad));
            _seq.Join(_hudCanvasGroup.DOFade(0f, _letterboxInDuration));

            yield return _seq.WaitForCompletion();

            _glitchOverlay.SetIntensity(_glitchIntensityStart);
            _glitchOverlay.StartGlitch();
            if (_glitchSfxKey.IsValid) SoundManager.Instance?.PlaySFX2D(_glitchSfxKey);

            _textCanvasGroup.alpha = 1f;
            _logText.text = "";

            string[] lines =
            {
                "접속 횟수: 999",
                "",
                "장비 반응 정상",
                "구역 진입에 성공했다",
                "",
                "첫 번째 기억 — 학교",
                "구조를 파악하고 다음 구역으로의 경로를 찾는다.."
            };

            float totalLines = lines.Length;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (string.IsNullOrEmpty(line))
                {
                    _logText.text += "\n";
                    yield return new WaitForSecondsRealtime(_linePause * _emptyLinePauseRatio);
                    continue;
                }

                int charCount = 0;
                foreach (char c in line)
                {
                    _logText.text += c;
                    _logText.ForceMeshUpdate();

                    if (_typingSfxKey.IsValid && !char.IsWhiteSpace(c) && ++charCount % _typingSoundInterval == 0)
                    {
                        SoundManager.Instance?.PlaySFX2D(_typingSfxKey);
                    }

                    float delay = _typingSpeed;
                    if (c == '…' || c == '—')
                        delay *= _specialCharDelayMultiplier;
                    else if (c == '.')
                        delay *= _periodDelayMultiplier;

                    yield return new WaitForSecondsRealtime(delay);
                }

                _logText.text += "\n";

                _glitchOverlay.SetIntensity(Mathf.Lerp(_glitchIntensityStart, _glitchIntensityEnd, (i + 1) / totalLines));

                if (i < lines.Length - 1)
                    yield return new WaitForSecondsRealtime(_linePause);
            }

            yield return new WaitForSecondsRealtime(_linePause);

            _glitchOverlay.StopGlitch();
            _textCanvasGroup.alpha = 0f;

            _seq?.Kill();
            _seq = DOTween.Sequence().SetUpdate(true);
            _seq.Append(_topBar
                .DOSizeDelta(new Vector2(_topBar.sizeDelta.x, 0f), _letterboxOutDuration)
                .SetEase(Ease.InQuad));
            _seq.Join(_bottomBar
                .DOSizeDelta(new Vector2(_bottomBar.sizeDelta.x, 0f), _letterboxOutDuration)
                .SetEase(Ease.InQuad));

            yield return _seq.WaitForCompletion();

            yield return new WaitForSecondsRealtime(_preJumpScareDelay);

            _mannequin.SetActive(true);
            if (_jumpScareSfxKey.IsValid) SoundManager.Instance?.PlaySFX2D(_jumpScareSfxKey);

            yield return new WaitForSecondsRealtime(_mannequinExposure);

            _mannequin.SetActive(false);
            _blackoutCanvasGroup.alpha = 1f;

            yield return new WaitForSecondsRealtime(_blackoutDuration);

            _onSequenceEnd?.Invoke();
        }

        private void OnDestroy()
        {
            _seq?.Kill();
        }
    }
}