using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.UI.Dark;
using _02.Scripts._01.Core.SceneTransition.Domain;
using _02.Scripts._01.Core.SceneTransition.Manager;
using _02.Scripts.Core;

namespace _02.Scripts._01.Core.SceneTransition.Component
{
    public class LoadingUI : MonoBehaviour
    {
        [Header("Dark UI")]
        [SerializeField] private ModalWindowManager _modalWindow;

        [Header("Loading Content")]
        [SerializeField] private Slider _progressBar;

        private ISceneTransitionManager _sceneTransition;

        private void Start()
        {
            _sceneTransition = Managers.Get<ISceneTransitionManager>();
            _sceneTransition.OnTransitionStarted += Show;
            _sceneTransition.OnLoadProgress += UpdateProgress;
            _sceneTransition.OnTransitionCompleted += Hide;
        }

        public void Setup(SceneDataSO sceneData)
        {
            _modalWindow.description = sceneData.LoadingTip;
            _modalWindow.title = sceneData.DisplayText;
            _modalWindow.UpdateUI();
            _progressBar.value = 0f;
        }

        private void Show()
        {
            _modalWindow.ModalWindowIn();
        }

        private void UpdateProgress(float progress)
        {
            _progressBar.value = progress;
        }

        private void Hide()
        {
            _modalWindow.ModalWindowOut();
        }

        private void OnDestroy()
        {
            if (_sceneTransition == null) return;
            _sceneTransition.OnTransitionStarted -= Show;
            _sceneTransition.OnLoadProgress -= UpdateProgress;
            _sceneTransition.OnTransitionCompleted -= Hide;
        }
    }
}
