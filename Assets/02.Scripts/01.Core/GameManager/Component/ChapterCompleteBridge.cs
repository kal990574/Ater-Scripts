using _02.Scripts.Core.Domain;
using UnityEngine;

namespace _02.Scripts.Core.Component
{
    public class ChapterCompleteBridge : MonoBehaviour
    {
        private enum EEndAction
        {
            CompleteChapter,   // 다음 챕터로 이동
            ReturnToMainMenu,  // 메인메뉴 복귀 (프로토타입용)
        }

        [Header("End Action")]
        [SerializeField] private EEndAction _endAction = EEndAction.ReturnToMainMenu;

        [Header("Delay")]
        [Tooltip("트리거 후 씬 전환까지 지연 시간(초)")]
        [SerializeField, Min(0f)] private float _delay = 1.5f;

        private bool _triggered;

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

            switch (_endAction)
            {
                case EEndAction.CompleteChapter:
                    gameManager.CompleteChapter();
                    break;
                case EEndAction.ReturnToMainMenu:
                    gameManager.ReturnToMainMenu();
                    break;
            }
        }
    }
}