using UnityEngine;
using _02.Scripts.Core;
using _02.Scripts.Core.Domain;

namespace _02.Scripts.UI.Component
{
    public class MainMenuController : MonoBehaviour
    {
        private IGameManager _gameManager;

        private void Start()
        {
            _gameManager = Managers.Get<IGameManager>();
        }

        public void PlayGame()
        {
            _gameManager.LoadChapter(1);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
