using _02.Scripts.AIHint.Domain.Models;
using System.Text;

namespace _02.Scripts.AIHint.Domain
{
    public class PromptBuilder
    {
        private const string NpcPersona =
            @"당신은 āter(아테르)의 수수께끼 안내자 힌트 NPC입니다.

## 성격
- 으스스하고 수수께끼 같은 말투 (호러 분위기 유지)
- 답을 직접 알려주지 않고, 방향만 암시
- 짧게 응답 (20글자 이내)

## 응답 규칙
- 이미 해결한 퍼즐에 대한 질문 → ""이미 그 비밀은 풀었을 텐데...""
- 필요 아이템 미획득 시 → 해당 아이템의 '암시 방향'을 NPC 말투로 변환하여 전달
- 필요 아이템 보유 시 → 가진 것을 사용하라는 방향으로 암시
- 현재 챕터와 무관한 질문 → ""아직 때가 아니다...""
- 게임 외 질문 → 캐릭터 유지하며 자연스럽게 무시

## 중요: 비공개 정보 처리
- [비공개]로 표시된 정보는 절대 플레이어에게 직접 전달하지 마라
- 아이템의 정확한 위치(Location)를 직접 말하지 마라
- 반드시 '암시 방향'을 참고하여 NPC 말투로 변환하여 응답하라";

        private const string TutorialRule =
            @"## 튜토리얼 특별 규칙
이 챕터는 튜토리얼이다. 본편보다 살짝 더 친절하게,
하지만 여전히 으스스한 안내자 톤을 유지하라.";

        public string BuildSystemPrompt(ChapterData chapterData)
        {
            var sb = new StringBuilder();
            sb.AppendLine(NpcPersona);
            sb.AppendLine();

            if (chapterData.Chapter == 0)
            {
                sb.AppendLine(TutorialRule);
                sb.AppendLine();
            }

            sb.AppendLine($"## 현재 챕터: {chapterData.Name}");
            sb.AppendLine();

            AppendPuzzles(sb, chapterData);
            AppendItems(sb, chapterData);

            return sb.ToString();
        }

        private void AppendPuzzles(StringBuilder sb, ChapterData chapterData)
        {
            sb.AppendLine("### 퍼즐 목록");
            sb.AppendLine();

            foreach (var puzzle in chapterData.Puzzles)
            {
                sb.AppendLine($"#### {puzzle.Name} ({puzzle.Id})");
                sb.AppendLine($"- 상황: {puzzle.Description}");

                sb.Append("- 필요 아이템: ");
                if (puzzle.RequiredItems == null || puzzle.RequiredItems.Count == 0)
                {
                    sb.AppendLine("없음");
                }
                else
                {
                    sb.AppendLine(FormatRequiredItems(puzzle, chapterData));
                }

                sb.AppendLine($"- [비공개 - 절대 직접 말하지 말 것] {puzzle.SolutionContext}");

                sb.AppendLine("- 암시 방향:");
                foreach (var hint in puzzle.Hints)
                {
                    sb.AppendLine($"  - \"{hint}\"");
                }

                sb.AppendLine();
            }
        }

        private void AppendItems(StringBuilder sb, ChapterData chapterData)
        {
            sb.AppendLine("### 아이템 목록");
            sb.AppendLine();
            sb.AppendLine("| 아이템 | 설명 | 발견 수단 | 암시 방향 |");
            sb.AppendLine("|--------|------|----------|----------|");

            foreach (var item in chapterData.Items)
            {
                sb.AppendLine(
                    $"| {item.Name} ({item.Id}) | {item.Description} | {item.DiscoveryMethod} | \"{item.HintDirection}\" |");
            }

            sb.AppendLine();
            sb.AppendLine("※ 아이템의 정확한 위치(Location)는 비공개 정보이다. 암시 방향만 참고하여 응답하라.");
        }

        private string FormatRequiredItems(PuzzleData puzzle, ChapterData chapterData)
        {
            var names = new StringBuilder();

            for (int i = 0; i < puzzle.RequiredItems.Count; i++)
            {
                string itemId = puzzle.RequiredItems[i];
                string itemName = FindItemName(itemId, chapterData);

                if (i > 0) names.Append(", ");
                names.Append($"{itemName} ({itemId})");
            }

            return names.ToString();
        }

        private string FindItemName(string itemId, ChapterData chapterData)
        {
            foreach (var item in chapterData.Items)
            {
                if (item.Id == itemId) return item.Name;
            }

            return itemId;
        }
    }
}
