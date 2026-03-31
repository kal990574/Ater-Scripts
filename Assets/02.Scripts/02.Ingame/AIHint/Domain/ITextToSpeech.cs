using Cysharp.Threading.Tasks;

namespace _02.Scripts.AIHint.Domain
{
    public interface ITextToSpeech
    {
        UniTask<byte[]> SynthesizeAsync(string text);
    }
}