using UnityEngine;                                          
using _02.Scripts.Core.Domain;
using _02.Scripts.UI.Domain;
using _02.Scripts.UI.Manager;
namespace _02.Scripts.Core.Presentation
{
    [DefaultExecutionOrder(-50)]
    public class InGameBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            var gameManager = ServiceLocator.Get<IGameManager>();
            var uiManager = new UIManager(gameManager);
            ServiceLocator.Register<IUIManager>(uiManager);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<IUIManager>();
        }
    }
}