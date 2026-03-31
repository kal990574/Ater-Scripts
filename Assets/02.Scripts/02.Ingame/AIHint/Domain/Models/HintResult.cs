namespace _02.Scripts.AIHint.Domain.Models
{
    public sealed class HintResult
    {
        public string HintText { get; }
        public byte[] AudioData { get; }
        public bool IsSuccess { get; }

        public HintResult(string hintText, byte[] audioData, bool isSuccess)
        {
            HintText = hintText;
            AudioData = audioData;
            IsSuccess = isSuccess;
        }

        public static HintResult Fail(string errorMessage)
        {
            return new HintResult(errorMessage, null, false);
        }
    }
}