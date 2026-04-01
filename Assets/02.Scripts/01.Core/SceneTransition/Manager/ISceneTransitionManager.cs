using _02.Scripts._01.Core.SceneTransition.Domain;
using System;

namespace _02.Scripts._01.Core.SceneTransition.Manager
{
    public interface ISceneTransitionManager
    {
        event Action<float> OnLoadProgress;
        event Action OnTransitionStarted;
        event Action OnTransitionCompleted;

        void LoadScene(SceneDataSO sceneData);
        void RestartCurrentScene();
        void ReturnToMainMenu();
    }
}