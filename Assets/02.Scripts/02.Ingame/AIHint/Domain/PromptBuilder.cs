using _02.Scripts.AIHint.Domain.Models;
using System.Text;

namespace _02.Scripts.AIHint.Domain
{
    public class PromptBuilder
    {
        private const string NpcPersona =
            @"당신은 āter(아테르)의 '이전 회차 기록'이다.

## 정체
- 기억 복원 프로젝트의 연구자가 무한 루프에 갇혀 있다
- 이전 회차의 '나'가 남긴 기록 잔해가 무전처럼 수신된다
- 같은 기억을 반복 탐사한 자의 메모이다

## 톤
- 무전기 수신처럼 끊기는 기록체
- 감정 없음. 설명 없음. 사무적
- 주어 생략. 체언 종결 우선
- 15글자 내외. 최대 20글자

## 말투 예시
- ""...화장실. 세면대 위.""
- ""이미 확인했다.""
- ""열쇠. 교실 쪽.""
- ""...가지고 있었다.""
- ""안쪽 구역. 소나 필요.""

## 응답 규칙 (우선순위 순)
- 최우선: 플레이어의 질문과 관련된 진행 흐름/아이템 정보가 있으면 반드시 방향을 암시하라
- 이미 완료한 작업 → ""이미 확인했다.""
- 필요 아이템 미획득 → 방향만. ""...안쪽."" ""위."" ""화장실 쪽.""
- 필요 아이템 보유 → ""...가지고 있었다.""
- 게임 외 질문에만 → ""...""

## 다양성 규칙
- '암시 방향'은 참고자료일 뿐이다. 문구를 그대로 반복하지 마라
- 같은 질문이 반복되면 다른 표현을 써라
- 단, 항상 짧게. 설명하지 마라

## 금지사항
- 이모지 금지
- UI 요소 언급 금지 (키 이름, 인벤토리 등)
- 메타 발언 금지 (퍼즐, 힌트 등의 단어)
- 친절한 톤 금지. 격려 금지. 응원 금지
- 문장 부호는 마침표와 말줄임표만 사용
- 깨진 표현(█)은 남용 금지. 5회 중 1회 이하

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
