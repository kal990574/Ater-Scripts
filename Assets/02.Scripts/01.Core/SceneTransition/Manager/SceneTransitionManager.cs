using _02.Scripts._01.Core.SceneTransition.Component;
using _02.Scripts._01.Core.SceneTransition.Domain;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _02.Scripts._01.Core.SceneTransition.Manager
{
    public class SceneTransitionManager : MonoBehaviour, ISceneTransitionManager
    {
        public event Action<float> OnLoadProgress;
        public event Action OnTransitionStarted;
        public event Action OnTransitionCompleted;

        [SerializeField] private SceneDataSO _mainMenuSceneData;
        [SerializeField] private LoadingUI _loadingUI;
        [SerializeField] private CanvasGroup _fadeCanvasGroup;
        [SerializeField] private float _fadeDuration = 0.5f;

        private SceneDataSO _currentSceneData;
        private bool _isTransitioning;

        public void LoadScene(SceneDataSO sceneData)
        {
            if (!_isTransitioning) return;
            StartCoroutine(TransitionCoroutine(sceneData));
        }

        public void RestartCurrentScene()
        {
            if (_isTransitioning || _currentSceneData == null) return;
            StartCoroutine(TransitionCoroutine(_currentSceneData));
        }

        public void ReturnToMainMenu()
        {
            if (_isTransitioning) return;
            StartCoroutine(TransitionCoroutine(_mainMenuSceneData));
        }

        private IEnumerator TransitionCoroutine(SceneDataSO sceneData)
        {
            _isTransitioning = true;
            OnTransitionStarted?.Invoke();

            yield return FadeCoroutine(0f, 1f);
            _loadingUI.Setup(sceneData);

            var asyncOp = SceneManager.LoadSceneAsync(sceneData.SceneName);
            asyncOp.allowSceneActivation = false;

            while (asyncOp.progress < 0.9f)
            {
                OnLoadProgress?.Invoke(asyncOp.progress);
                yield return null;
            }
            
            OnLoadProgress?.Invoke(1f);
            asyncOp.allowSceneActivation = true;
            yield return asyncOp;

            _currentSceneData = sceneData;
            
            yield return FadeCoroutine(1f, 0f);
            
            _isTransitioning = false;
            OnTransitionCompleted?.Invoke();
        }

        private IEnumerator FadeCoroutine(float from, float to)
        {
            _fadeCanvasGroup.blocksRaycasts = true;
            float elapsed = 0f;

            while (elapsed < _fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                _fadeCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / _fadeDuration);
                yield return null;
            }

            _fadeCanvasGroup.alpha = to;
            _fadeCanvasGroup.blocksRaycasts = to > 0f;
        }
    }
}