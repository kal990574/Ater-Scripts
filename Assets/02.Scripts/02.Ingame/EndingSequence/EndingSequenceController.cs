using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace _02.Scripts._02.Ingame.EndingSequence
{
    public class EndingSequenceController : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private PlayerController _playerController;

        [Header("Letterbox")]
        [SerializeField] private RectTransform _topBar;
        [SerializeField] private RectTransform _bottomBar;
        [SerializeField] private float _letterboxHeight = 120f;
        [SerializeField] private float _letterboxDuration = 0.8f;
        [SerializeField] private CanvasGroup _hudCanvasGroup;

        [Header("Glitch")]
        [SerializeField] private GlitchOverlayUI _glitchOverlay;

        [Header("Text UI")]
        [SerializeField] private CanvasGroup _textCanvasGroup;
        [SerializeField] private TMP_Text _logText;

        [Header("Mannequin")]
        [SerializeField] private GameObject _mannequin;

        [Header("Blackout")]
        [SerializeField] private CanvasGroup _blackoutCanvasGroup;

        [Header("Timing")]
        [SerializeField] private float _freeRoamDuration = 7f;
        [SerializeField] private float _typingSpeed = 0.04f;
        [SerializeField] private float _linePause = 1.0f;
        [SerializeField] private float _mannequinExposure = 0.15f;

        [Header("Audio")]
        [SerializeField] private AudioSource _glitchSfx;
        [SerializeField] private AudioSource _jumpScareSfx;

        [Header("Events")]
        [SerializeField] private UnityEvent _onSequenceEnd;

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
            // 1. 자유 탐색 시간
            yield return new WaitForSecondsRealtime(_freeRoamDuration);

            // 2. 조작 잠금 + 레터박스
            _playerController.EnterCutsceneMode();
            _hudAnimator = _hudCanvasGroup.GetComponent<Animator>();
            if (_hudAnimator != null) _hudAnimator.enabled = false;

            _seq = DOTween.Sequence().SetUpdate(true);
            _seq.Append(_topBar
                .DOSizeDelta(new Vector2(_topBar.sizeDelta.x, _letterboxHeight), _letterboxDuration)
                .SetEase(Ease.OutQuad));
            _seq.Join(_bottomBar
                .DOSizeDelta(new Vector2(_bottomBar.sizeDelta.x, _letterboxHeight), _letterboxDuration)
                .SetEase(Ease.OutQuad));
            _seq.Join(_hudCanvasGroup.DOFade(0f, _letterboxDuration));

            yield return _seq.WaitForCompletion();

            // 3. 글리치 시작 (약하게) + 사운드
            _glitchOverlay.SetIntensity(0.3f);
            _glitchOverlay.StartGlitch();
            if (_glitchSfx != null) _glitchSfx.Play();

            // 4. 텍스트 타이핑 (줄별 표시)
            _textCanvasGroup.alpha = 1f;
            _logText.text = "";

            string[] lines =
            {
                "접속 횟수: 99.",
                "장비 반응 정상. 구역 진입에 성공했다.",
                "",
                "첫 번째 기억 — 학교.",
                "구조를 파악하고 다음 구역으로의 경로를 찾는다."
            };

            float totalLines = lines.Length;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (string.IsNullOrEmpty(line))
                {
                    _logText.text += "\n";
                    yield return new WaitForSecondsRealtime(_linePause * 0.5f);
                    continue;
                }

                foreach (char c in line)
                {
                    _logText.text += c;
                    _logText.ForceMeshUpdate();
                    yield return new WaitForSecondsRealtime(_typingSpeed);
                }

                _logText.text += "\n";

                // 글리치 강도를 줄 진행에 따라 올림
                _glitchOverlay.SetIntensity(Mathf.Lerp(0.3f, 1f, (i + 1) / totalLines));

                if (i < lines.Length - 1)
                    yield return new WaitForSecondsRealtime(_linePause);
            }

            // 마지막 줄 후 잠시 대기
            yield return new WaitForSecondsRealtime(_linePause);

            // 5. 글리치 정지 + 텍스트 숨김
            _glitchOverlay.StopGlitch();
            if (_glitchSfx != null) _glitchSfx.Stop();
            _textCanvasGroup.alpha = 0f;

            // 6. 레터박스 아웃
            _seq?.Kill();
            _seq = DOTween.Sequence().SetUpdate(true);
            _seq.Append(_topBar
                .DOSizeDelta(new Vector2(_topBar.sizeDelta.x, 0f), _letterboxDuration)
                .SetEase(Ease.InQuad));
            _seq.Join(_bottomBar
                .DOSizeDelta(new Vector2(_bottomBar.sizeDelta.x, 0f), _letterboxDuration)
                .SetEase(Ease.InQuad));

            yield return _seq.WaitForCompletion();

            // 7. 정적 후 마네킹 점프스케어
            yield return new WaitForSecondsRealtime(2f);

            _mannequin.SetActive(true);
            if (_jumpScareSfx != null) _jumpScareSfx.Play();

            yield return new WaitForSecondsRealtime(_mannequinExposure);

            // 8. 암전
            _mannequin.SetActive(false);
            _blackoutCanvasGroup.alpha = 1f;

            yield return new WaitForSecondsRealtime(1.5f);

            // 9. 엔딩 이벤트
            _onSequenceEnd?.Invoke();
        }

        private void OnDestroy()
        {
            _seq?.Kill();
        }
    }
}