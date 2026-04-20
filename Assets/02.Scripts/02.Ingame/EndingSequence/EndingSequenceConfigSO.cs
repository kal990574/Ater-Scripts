using UnityEngine;

namespace _02.Scripts._02.Ingame.EndingSequence
{
    [CreateAssetMenu(fileName = "EndingSequenceConfig", menuName = "Ater/Ending Sequence Config")]
    public class EndingSequenceConfigSO : ScriptableObject
    {
        [SerializeField, TextArea(1, 2), Tooltip("엔딩 로그 텍스트 (줄 단위, 빈 줄은 빈 문자열)")]
        private string[] _lines =
        {
            "접속 횟수: 999",
            "",
            "장비 반응 정상",
            "구역 진입에 성공했다",
            "",
            "첫 번째 기억 — 학교",
            "구조를 파악하고 다음 구역으로의 경로를 찾는다.."
        };

        public string[] Lines => _lines;
    }
}