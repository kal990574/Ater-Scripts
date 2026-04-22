using UnityEngine;

namespace _02.Scripts._02.Ingame.EndingSequence
{
    [CreateAssetMenu(fileName = "EndingSequenceConfig", menuName = "Ater/Ending Sequence Config")]
    public class EndingSequenceConfigSO : ScriptableObject
    {
        [SerializeField, TextArea(5, 20), Tooltip("엔딩 로그 텍스트 (줄바꿈으로 구분)")]
        private string _text;

        public string Text => _text;
    }
}