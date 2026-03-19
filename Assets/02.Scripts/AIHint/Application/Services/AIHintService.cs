using _02.Scripts.AIHint.Domain;
using _02.Scripts.AIHint.Domain.Models;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _02.Scripts.AIHint.Application.Services
{
    public class AIHintService
    {
        private readonly ISpeechToText _stt;
        private readonly ILanguageModel _llm;

        public ILanguageModel Llm => _llm;

        public AIHintService(ISpeechToText stt, ILanguageModel llm)
        {
            _stt = stt;
            _llm = llm;
        }

        public async UniTask<HintResponse> ProcessHintAsync(
            byte[] audioData,
            PlayerHintState playerState)
        {
            string userQuery = await _stt.RecognizeAsync(audioData);
            Debug.Log($"[AIHint] STT 결과: {userQuery}");

            var request = new HintRequest(userQuery, playerState);
            HintResponse response = await _llm.GenerateHintAsync(request);

            if (response.IsSuccess)
            {
                Debug.Log($"[AIHint] 힌트: {response.HintText}");
            }
            else
            {
                Debug.LogWarning($"[AIHint] LLM 실패: {response.HintText}");
            }

            return response;
        }
    }
}