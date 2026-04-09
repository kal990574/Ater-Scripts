using UnityEngine;

namespace _02.Scripts._01.Core.SceneTransition.Domain
{
    [CreateAssetMenu(fileName = "SceneData", menuName = "Ater/SceneData")]
    public class SceneDataSO : ScriptableObject
    {
        [Header("Scene Info")]
        [SerializeField] private string _sceneName;
        [SerializeField] private int _chapterId;
        
        [Header("BGM Info")]
        [SerializeField] private SoundKeyReference _bgmKey;

        [Header("Loading Screen")]
        [SerializeField] private string _displayText;
        [SerializeField] [TextArea] private string _loadingTip;

        [Header("Narration")]
        [SerializeField] private bool _hasNarration;
        [SerializeField] [TextArea(5, 20)] private string _narrationText;
        [SerializeField] private float _typingSpeed = 0.04f;

        public string SceneName => _sceneName;
        public int ChapterId => _chapterId;
        public string DisplayText => _displayText;
        public string LoadingTip => _loadingTip;
        public string BgmKey => _bgmKey;
        public bool HasNarration => _hasNarration;
        public string NarrationText => _narrationText;
        public float TypingSpeed => _typingSpeed;
    }
}
