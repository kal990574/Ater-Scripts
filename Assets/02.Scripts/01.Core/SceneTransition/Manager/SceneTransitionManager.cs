using _02.Scripts._01.Core.SceneTransition.Component;
using _02.Scripts._01.Core.SceneTransition.Domain;
using _02.Scripts.Core;
using _02.Scripts.Core.Domain;
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
        [SerializeField] private NarrationUI _narrationUI;

        private SceneDataSO _currentSceneData;
        private bool _isTransitioning;
        private IGameManager _gameManager;

        private void Start()
        {
            _gameManager = Managers.Get<IGameManager>();

            if (!string.IsNullOrEmpty(_mainMenuSceneData.BgmKey) && SoundManager.Instance != null)
                SoundManager.Instance.PlayBGM(_mainMenuSceneData.BgmKey);
        }

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

            // GameState → Transitioning (timeScale=0, PauseAll, 이벤트 발행)
            _gameManager.EnterTransition();

            // BGM 정지
            if (SoundManager.Instance != null)
                SoundManager.Instance.StopBGM(0.5f);

            // 나레이션
            if (sceneData.HasNarration && _narrationUI != null)
                yield return _narrationUI.PlayNarration(sceneData);

            // 나레이션 끝난 후 로딩 시작
            OnTransitionStarted?.Invoke();
            _loadingUI.Setup(sceneData);

            yield return new WaitForSecondsRealtime(_modalAnimationDelay);

            // 로딩 화면 애니메이션 완료 후 나레이션 배경 해제
            if (_narrationUI != null)
                _narrationUI.Hide();

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
                SoundManager.Instance.PlayBGM(sceneData.BgmKey);

            // GameState → Playing (timeScale=1, ResumeAll, 이벤트 발행)
            _gameManager.ExitTransition();
            _isTransitioning = false;
            OnTransitionCompleted?.Invoke();
        }
    }
}
