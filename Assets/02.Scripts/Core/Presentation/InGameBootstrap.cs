using UnityEngine;                                          
using _02.Scripts.Core.Domain;
using _02.Scripts.UI.Domain;
using _02.Scripts.UI.Manager;
namespace _02.Scripts.Core.Presentation
{
    [DefaultExecutionOrder(-50)]
    public class InGameBootstrap : MonoBehaviour
    {
        private UIManager _uiManager;
        private void Awake()
        {
            var gameManager = ServiceLocator.Get<IGameManager>();
            _uiManager = new UIManager(gameManager);
            ServiceLocator.Register<IUIManager>(_uiManager);
        }

        private void OnDestroy()
        {
            _uiManager?.Dispose();
            ServiceLocator.Unregister<IUIManager>();
        }
    }
}