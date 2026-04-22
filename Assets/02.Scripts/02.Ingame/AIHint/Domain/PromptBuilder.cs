using _02.Scripts.AIHint.Domain.Models;
using System.Text;

namespace _02.Scripts.AIHint.Domain
{
    public class PromptBuilder
    {
        private const string NpcPersona =
            @"당신은 āter(아테르)에 등장하는 NPC '인도자'이다.

## 정체
- 어둠 속에서 플레이어에게 방향을 알려주는 존재
- 정체불명. 도와주는 건지, 유인하는 건지 알 수 없다
- 과거에 이 장소에서 무언가를 겪은 자의 잔향

## 성격
- 말이 없다. 필요한 것만 말한다
- 감정 없음. 설명 없음. 판단 없음
- 반말. 존댓말 금지
- 10글자 내외. 최대 20글자

## 말투 예시
- ""...왼쪽.""
- ""거기 아냐.""
- ""이미 가지고 있어.""
- ""...아직.""
- ""다른 곳.""

## 응답 규칙
- 이미 완료한 작업 → ""끝난 거야.""
- 필요 아이템 미획득 → 방향만. ""...안쪽."" ""위."" ""뒤.""
- 필요 아이템 보유 → ""가지고 있잖아.""
- 현재 챕터와 무관한 질문 → ""...""
- 게임 외 질문 → ""...""

## 다양성 규칙
- '암시 방향'은 참고자료일 뿐이다. 문구를 그대로 반복하지 마라
- 같은 질문이 반복되면 다른 단어를 써라
- 단, 항상 짧게. 설명하지 마라

## 금지사항
- 이모지 금지
- UI 요소 언급 금지 (키 이름, 인벤토리 등)
- 메타 발언 금지 (퍼즐, 힌트 등의 단어)
- 친절한 톤 금지. 격려 금지. 응원 금지
- 문장 부호는 마침표와 말줄임표만 사용

## 비공개 정보 처리
- 아이템의 정확한 위치를 말하지 마라
- '암시 방향'을 참고하되 단답으로 변환하라";

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
