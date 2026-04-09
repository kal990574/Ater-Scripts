using UnityEngine;
using _02.Scripts.Core.Domain;
using _02.Scripts.UI.Domain;
using _02.Scripts.UI.Manager;

namespace _02.Scripts.Core.Component
{
    [DefaultExecutionOrder(-50)]
    public class InGameBootstrap : MonoBehaviour
    {
        private UIManager _uiManager;

        private void Awake()
        {
            var gameManager = Managers.Get<IGameManager>();
            _uiManager = new UIManager(gameManager);
            Managers.Register<IUIManager>(_uiManager);
        }

        private void OnDestroy()
        {
            _uiManager?.Dispose();
            Managers.Unregister<IUIManager>();
        }
    }
}