using UnityEngine;
using UnityEngine.UI;
using TMPro;
using _02.Scripts._01.Core.SceneTransition.Domain;
using _02.Scripts._01.Core.SceneTransition.Manager;
using _02.Scripts.Core;

namespace _02.Scripts._01.Core.SceneTransition.Component
{
    public class LoadingUI : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Slider _progressBar;
        [SerializeField] private TMP_Text _tipText;
        [SerializeField] private TMP_Text _displayNameText;

        private ISceneTransitionManager _sceneTransition;

        private void Start()
        {
            _sceneTransition = Managers.Get<ISceneTransitionManager>();
            _sceneTransition.OnTransitionStarted += Show;
            _sceneTransition.OnLoadProgress += UpdateProgress;
            _sceneTransition.OnTransitionCompleted += Hide;
            
            _canvas.enabled = false;
        }

        public void Setup(SceneDataSO sceneData)
        {
            if(sceneData.LoadingImage != null) _backgroundImage.sprite = sceneData.LoadingImage;
            
            _tipText.text = sceneData.LoadingTip;
            _displayNameText.text = sceneData.DisplayName;
            _progressBar.value = 0f;
        }

        private void Show()
        {
            _canvas.enabled = true;
        }

        private void UpdateProgress(float progress)
        {
            _progressBar.value = progress;
        }

        private void Hide()
        {
            _canvas.enabled = false;
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