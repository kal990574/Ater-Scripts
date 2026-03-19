using _02.Scripts.AIHint.Domain.Models;
using Cysharp.Threading.Tasks;

namespace _02.Scripts.AIHint.Domain
{
    public interface ILanguageModel
    {
        UniTask<HintResponse> GenerateHintAsync(HintRequest request);
    }
}