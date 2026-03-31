using UnityEngine;

namespace _02.Scripts._01.Core.SceneTransition.Domain
{
    [CreateAssetMenu(fileName = "SceneData", menuName = "Ater/SceneData")]
    public class SceneDataSO : ScriptableObject
    {
        [Header("Scene Info")]
        [SerializeField] private string _sceneName;

        [SerializeField] private int _chapterId;
        
        [Header("Loading Screen")]
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _loadingImage;
        [SerializeField] [TextArea] private string _loadingTip;
        
        public string SceneName => _sceneName;
        public int ChapterId => _chapterId;
        public string DisplayName => _displayName;
        public Sprite LoadingImage => _loadingImage;
        public string LoadingTip => _loadingTip;
    }
}