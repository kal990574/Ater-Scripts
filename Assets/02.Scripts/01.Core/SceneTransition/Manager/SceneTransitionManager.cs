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
            _loadingUI.Setup(sceneData);
            
            // TODO: fade out 효과 등
            yield return new WaitForSecondsRealtime(0.5f);

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
            
            // TODO: fade in 효과 등
            yield return new WaitForSecondsRealtime(0.5f);
            
            _isTransitioning = false;
            OnTransitionCompleted?.Invoke();
        }
    }
}