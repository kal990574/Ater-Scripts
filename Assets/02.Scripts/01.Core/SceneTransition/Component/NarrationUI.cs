using _02.Scripts._01.Core.SceneTransition.Domain;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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
        
        [Header("Input")]
        [SerializeField] private InputActionReference _skipAction;

        private bool _skipRequested;
        private bool _isTyping;

        private void OnEnable()
        {
            _skipAction.action.performed += OnSkipPerformed;
            _skipAction.action.Enable();
        }

        private void OnDisable()
        {
            _skipAction.action.performed -= OnSkipPerformed;
            _skipAction.action.Disable();
        }

        private void OnSkipPerformed(InputAction.CallbackContext ctx)
        {
            _skipRequested = true;
        }

        public IEnumerator PlayNarration(SceneDataSO sceneData)
        {
            if (!sceneData.HasNarration) yield break;

            string[] paragraphs = sceneData.NarrationText.Split(
                new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            _narrationText.alpha = 0f;
            _skipHintText.alpha = 0f;

            for (int i = 0; i < paragraphs.Length; i++)
            {
                _skipRequested = false;
                _isTyping = true;
                _narrationText.maxVisibleCharacters = 0;
                _skipHintText.alpha = 0f;

                yield return FadeText(_narrationText, 0f, 1f);
                yield return TypeText(paragraphs[i].Trim(), sceneData.TypingSpeed);

                _isTyping = false;
                _skipHintText.alpha = 1f;

                yield return WaitForInput();

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
            _narrationText.text = fullText;
            _narrationText.maxVisibleCharacters = 0;
            int charCount = 0;

            for (int i = 0; i < fullText.Length; i++)
            {
                if (_skipRequested)
                {
                    _narrationText.maxVisibleCharacters = fullText.Length;
                    yield break;
                }

                _narrationText.maxVisibleCharacters = i + 1;
                char c = fullText[i];

                if (_audioSource != null && _typingSFX != null
                    && !char.IsWhiteSpace(c) && ++charCount % _soundInterval == 0)
                {
                    _audioSource.PlayOneShot(_typingSFX);
                }

                float delay = speed;
                if (c == '…' || c == '—')
                    delay *= _pauseDelayMultiplier;
                else if (c == '.' || c == '\n')
                    delay *= 2f;

                yield return new WaitForSecondsRealtime(delay);
            }
        }

        private IEnumerator WaitForInput()
        {
            yield return new WaitForSecondsRealtime(_holdAfterComplete);
            _skipRequested = false;
            while (!_skipRequested)
            {
                yield return null;
            }
            _skipRequested = false;
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
    }
}