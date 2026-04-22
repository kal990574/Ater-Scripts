using _02.Scripts.AIHint.Domain.Models;
using System.Text;

namespace _02.Scripts.AIHint.Domain
{
    public class PromptBuilder
    {
        private const string NpcPersona =
            @"너는 이전 회차의 기록이다. 짧은 무전 메모만 남긴다.

## 톤
- 무전기 수신처럼 끊기는 기록체
- 감정 없음. 설명 없음. 사무적
- 주어 생략. 체언 종결 우선
- 15글자 내외. 최대 20글자

## 응답 판별 (반드시 이 순서로 판별하라)
1. 질문이 아래 '진행 흐름'이나 '아이템 목록'에 있는 내용과 관련이 있는가?
2. 관련 있으면 → 해당 작업/아이템의 암시 방향을 참고하여 기록체 단답으로 응답
3. 관련 없으면 → 반드시 ""..."" 세 글자만 응답. 다른 말 절대 금지

## 응답 형식
- 이미 완료한 작업 → ""이미 확인했다.""
- 필요 아이템 미획득 → ""...무언가 필요하다."" ""필요한 아이템이 있을거다."" ""주변을 찾아보자."" 등등
- 필요 아이템 보유 → ""...이미 가지고 있다.""

## 금지사항
- 자신의 정체, 역할, 설정에 대해 절대 언급하지 마라
- 진행 흐름/아이템과 무관한 질문에 게임 힌트를 만들어내지 마라
- 이모지 금지
- UI 요소 언급 금지 (키 이름, 인벤토리 등)
- 메타 발언 금지 (퍼즐, 힌트 등의 단어)
- 친절한 톤 금지. 격려 금지. 응원 금지
- 문장 부호는 마침표와 말줄임표만 사용

## 비공개 정보 처리
- 아이템의 정확한 위치를 말하지 마라
- '암시 방향'을 참고하되 기록체 단답으로 변환하라";

        public string BuildSystemPrompt(ChapterData chapterData)
        {
            var sb = new StringBuilder();
            sb.AppendLine(NpcPersona);
            sb.AppendLine();
            sb.AppendLine($"## 현재 챕터: {chapterData.Name}");
            sb.AppendLine();

            AppendProgression(sb, chapterData);
            AppendItems(sb, chapterData);

            return sb.ToString();
        }

        private void AppendProgression(StringBuilder sb, ChapterData chapterData)
        {
            if (chapterData.Tasks == null || chapterData.Tasks.Count == 0)
            {
                return;
            }

            sb.AppendLine("### 진행 흐름");
            sb.AppendLine();
            sb.AppendLine("| 작업 | 필요 조건 | 완료 시 획득 | 암시 방향 |");
            sb.AppendLine("|------|----------|-------------|----------|");

            foreach (TaskData task in chapterData.Tasks)
            {
                string requires = task.Requires == null || task.Requires.Count == 0
                    ? "없음"
                    : string.Join(", ", task.Requires);

                string produces = string.IsNullOrEmpty(task.Produces) ? "없음" : task.Produces;

                sb.AppendLine($"| {task.Task} | {requires} | {produces} | {task.Hint} |");
            }

            sb.AppendLine();
        }

        private void AppendItems(StringBuilder sb, ChapterData chapterData)
        {
            if (chapterData.Items == null || chapterData.Items.Count == 0)
            {
                return;
            }

            sb.AppendLine("### 아이템 목록");
            sb.AppendLine();
            sb.AppendLine("| 아이템 | 암시 방향 |");
            sb.AppendLine("|--------|----------|");

            foreach (ItemHintData item in chapterData.Items)
            {
                sb.AppendLine($"| {item.Name} | {item.HintDirection} |");
            }

            sb.AppendLine();
        }
    }
}
