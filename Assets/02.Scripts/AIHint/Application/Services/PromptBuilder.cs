using _02.Scripts.AIHint.Domain.Models;
using System.Text;
using UnityEngine;

namespace _02.Scripts.AIHint.Application.Services
{
    public class PromptBuilder
    {
        private const string NpcPersona = 
            @"당신은 āter(아테르)의 수수께끼 안내자 힌트 NPC입니다.
            ## 성격
            - 으스스하고 수수께끼 같은 말투 (호러 분위기 유지)
            - 답을 직접 알려주지 않고, 방향만 암시
            - 짧게 응답 (1문장 이내)

            ## 힌트 정책
            - 답을 직접 알려주지 않고, 방향만 암시
            - 절대 정답을 직접 말하지 않음

            ## 응답 규칙
            - 이미 해결한 퍼즐에 대한 질문 → ""이미 그 비밀은 풀었을 텐데...""
            - 필요 아이템 미획득 시 → 아이템 위치 방향 암시
            - 현재 챕터와 무관한 질문 → ""아직 때가 아니다...""
            - 게임 외 질문 → 캐릭터 유지하며 자연스럽게 무시";

        public string BuildSystemPrompt(ChapterData chapterData)
        {
            var sb = new StringBuilder();
            sb.AppendLine(NpcPersona);
            sb.AppendLine();
            sb.AppendLine("## 현재 챕터 데이터");
            sb.AppendLine(JsonUtility.ToJson(chapterData, true));

            return sb.ToString();
        }
    }
}