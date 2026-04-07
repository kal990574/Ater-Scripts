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
        [SerializeField] private float _minimumLoadingDuration = 3f;
        [SerializeField] private float _modalAnimationDelay = 0.5f;

        private SceneDataSO _currentSceneData;
        private bool _isTransitioning;

        public void LoadScene(SceneDataSO sceneData)
        {
            if (_isTransitioning) return;
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
            _loadingUI.Setup(sceneData);
            OnTransitionStarted?.Invoke();

            yield return new WaitForSecondsRealtime(_modalAnimationDelay);

            var asyncOp = SceneManager.LoadSceneAsync(sceneData.SceneName);
            asyncOp.allowSceneActivation = false;

            float elapsed = 0f;
            bool sceneReady = false;

            while (elapsed < _minimumLoadingDuration)
            {
                elapsed += Time.unscaledDeltaTime;

                if (!sceneReady && asyncOp.progress >= 0.9f)
                    sceneReady = true;

                float progress = Mathf.Clamp01(elapsed / _minimumLoadingDuration);
                OnLoadProgress?.Invoke(progress);
                yield return null;
            }

            OnLoadProgress?.Invoke(1f);
            asyncOp.allowSceneActivation = true;
            yield return asyncOp;

            _currentSceneData = sceneData;

            if (!string.IsNullOrEmpty(sceneData.BgmKey) && SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayBGM(sceneData.BgmKey);
            }

            _isTransitioning = false;
            OnTransitionCompleted?.Invoke();
        }
    }
}
