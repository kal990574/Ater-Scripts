namespace _02.Scripts.AIHint.Domain.Models
{
    public sealed class HintResponse
    {
        public string HintText { get; }
        public bool IsSuccess { get; }

        public HintResponse(string hintText, bool isSuccess)
        {
            HintText = hintText;
            IsSuccess = isSuccess;
        }

        public static HintResponse Fail(string errorMessage)
        {
            return new HintResponse(errorMessage, false);
        }
    }
}