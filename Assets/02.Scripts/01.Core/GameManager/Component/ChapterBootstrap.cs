using UnityEngine;
using _02.Scripts.Core.Timer.Manager;

namespace _02.Scripts.Core.Component
{
    [DefaultExecutionOrder(-49)]
    public class ChapterBootstrap : MonoBehaviour
    {
        [Header("Timer")]
        [SerializeField] private ChapterTimerManager _timerManager;

        private void Awake()
        {
            Managers.Register<IChapterTimerManager>(_timerManager);
        }

        private void OnDestroy()
        {
            Managers.Unregister<IChapterTimerManager>();
        }
    }
}