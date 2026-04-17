using _02.Scripts.Core.Domain;
using UnityEngine;

namespace _02.Scripts.Core.Component
{
    public class ChapterCompleteBridge : MonoBehaviour
    {
        [Header("Delay")]
        [Tooltip("트리거 후 씬 전환까지 지연 시간(초)")]
        [SerializeField, Min(0f)] private float _delay = 1.5f;

        private readonly GameEventPublisher _gameEventPublisher = new GameEventPublisher();
        private bool _triggered;

        private void Awake()
        {
            _gameEventPublisher.SetSource(this);
        }

        public void Trigger()
        {
            if (_triggered) return;
            _triggered = true;

            if (_delay <= 0f)
            {
                Execute();
                return;
            }

            Invoke(nameof(Execute), _delay);
        }

        private void Execute()
        {
            var gameManager = Managers.Get<IGameManager>();
            if (gameManager == null)
            {
                Debug.LogError("[ChapterCompleteBridge] GameManager not found");
                return;
            }

            gameManager.CompleteChapter();
        }
        
        private void PublishChapterCleared(int chapterId)
        {
            _gameEventPublisher.TryPublish(context => new ChapterClearedRawEvent(context, chapterId));
        }
    }
}