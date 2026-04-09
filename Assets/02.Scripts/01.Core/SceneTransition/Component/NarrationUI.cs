using _02.Scripts._01.Core.SceneTransition.Domain;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace _02.Scripts._01.Core.SceneTransition.Component
{
    public class NarrationUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _narrationText;
        [SerializeField] private TMP_Text _skipHintText;
        
        [Header("Settings")]
        [SerializeField] private float _fadeDuration = 0.5f;
        [SerializeField] private float _holdAfterComplete = 1.5f;
        [SerializeField] private float _pauseDelayMultiplier = 3f;

        [Header("Sound")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _typingSFX;
        [SerializeField] private int _soundInterval = 2;

        private bool _skipRequested;
        private bool _isTyping;

        public IEnumerator PlayNarration(SceneDataSO sceneData)
        {
            if (!sceneData.HasNarration) yield break;

            if (_audioSource == null)
            {
                GameObject uiAudio = GameObject.Find("UI Audio");
                if (uiAudio != null && uiAudio.TryGetComponent<AudioSource>(out var source))
                    _audioSource = source;
            }

            string[] paragraphs = sceneData.NarrationText.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

            // 배경 즉시 표시 + 뒤쪽 UI 입력 차단
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            _narrationText.alpha = 0f;
            _skipHintText.alpha = 0f;

            for (int i = 0; i < paragraphs.Length; i++)
            {
                _skipRequested = false;
                _isTyping = true;
                _narrationText.text = "";
                _skipHintText.alpha = 0f;

                // 텍스트만 페이드 인
                yield return FadeText(_narrationText, 0f, 1f);
                yield return TypeText(paragraphs[i].Trim(), sceneData.TypingSpeed);

                _isTyping = false;
                _skipHintText.alpha = 1f;

                yield return WaitForInput();

                // 텍스트만 페이드 아웃
                _skipHintText.alpha = 0f;
                yield return FadeText(_narrationText, 1f, 0f);
            }
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
        }

        private IEnumerator TypeText(string fullText, float speed)
        {
            _narrationText.text = "";
            int charCount = 0;

            foreach (char c in fullText)
            {
                if (_skipRequested)
                {
                    _narrationText.text = fullText;
                    yield break;
                }

                _narrationText.text += c;

                if (_typingSFX != null && !char.IsWhiteSpace(c)
                    && ++charCount % _soundInterval == 0)
                {
                    _audioSource.PlayOneShot(_typingSFX);
                }

                float delay = speed;
                if (c == '…' || c == '—')
                {
                    delay *= _pauseDelayMultiplier;
                }
                else if (c == '.' || c == '\n')
                {
                    delay *= 2f;
                }

                yield return new WaitForSecondsRealtime(delay);
            }
        }

        private IEnumerator WaitForInput()
        {
            yield return new WaitForSecondsRealtime(_holdAfterComplete);
            while (!Input.anyKeyDown)
            {
                yield return null;
            }
        }

        private IEnumerator FadeText(TMP_Text text, float from, float to)
        {
            float elapsed = 0f;

            while (elapsed < _fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                text.alpha = Mathf.Lerp(from, to, elapsed / _fadeDuration);
                yield return null;
            }

            text.alpha = to;
        }

        private IEnumerator FadeCanvas(float from, float to)
        {
            float elapsed = 0f;

            while (elapsed < _fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                _canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / _fadeDuration);
                yield return null;
            }

            _canvasGroup.alpha = to;
        }

        private void Update()
        {
            if (_isTyping && !_skipRequested && Input.anyKeyDown)
            {
                _skipRequested = true;
            }
        }
    }
}