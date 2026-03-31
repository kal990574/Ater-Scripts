namespace _02.Scripts.AIHint.Domain.Models
{
    public sealed class HintRequest
    {
        public string UserQuery { get; }
        public PlayerHintState PlayerState { get; }

        public HintRequest(string userQuery, PlayerHintState playerState)
        {
            UserQuery = userQuery;
            PlayerState = playerState;
        }
    }
}