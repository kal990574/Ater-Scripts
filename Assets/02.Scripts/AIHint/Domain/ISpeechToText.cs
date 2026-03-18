using Cysharp.Threading.Tasks;

namespace _02.Scripts.AIHint.Domain
{
    public interface ISpeechToText
    {
        UniTask<string> RecognizeAsync(byte[] audioData);
    }
}