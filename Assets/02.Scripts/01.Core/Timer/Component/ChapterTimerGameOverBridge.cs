using _02.Scripts.Core.Domain;
using _02.Scripts.Core.Timer.Manager;
using UnityEngine;

namespace _02.Scripts.Core.Timer.Component
{
    [RequireComponent(typeof(ChapterTimerManager))]
    public class ChapterTimerGameOverBridge : MonoBehaviour
    {
        private IChapterTimerManager _timer;

        private void Awake()
        {
            _timer = GetComponent<ChapterTimerManager>();
        }

        private void OnEnable()
        {
            if (_timer != null)
                _timer.OnTimerExpired += HandleTimerExpired;
        }

        private void OnDisable()
        {
            if (_timer != null)
                _timer.OnTimerExpired -= HandleTimerExpired;
        }

        private void HandleTimerExpired()
        {
            var gameManager = Managers.Get<IGameManager>();
            gameManager.GameOver();
        }
    }
}
