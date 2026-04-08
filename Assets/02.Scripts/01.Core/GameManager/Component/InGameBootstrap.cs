using UnityEngine;
using _02.Scripts.Core.Domain;
using _02.Scripts.Core.Timer.Manager;
using _02.Scripts.UI.Domain;
using _02.Scripts.UI.Manager;
namespace _02.Scripts.Core.Component
{
    [DefaultExecutionOrder(-50)]
    public class InGameBootstrap : MonoBehaviour
    {
        // TODO: 씬 별 타이머 값 적용
        [Header("Timer")]
        [SerializeField] private float _timeLimitSeconds = 300f;

        private UIManager _uiManager;
        private ChapterTimerManager _timerManager;

        private void Awake()
        {
            var gameManager = Managers.Get<IGameManager>();
            _uiManager = new UIManager(gameManager);
            Managers.Register<IUIManager>(_uiManager);

            _timerManager = new ChapterTimerManager();
            Managers.Register<IChapterTimerManager>(_timerManager);
            _timerManager.StartTimer(_timeLimitSeconds);
        }

        private void OnDestroy()
        {
            _uiManager?.Dispose();
            Managers.Unregister<IUIManager>();
            Managers.Unregister<IChapterTimerManager>();
        }
    }
}