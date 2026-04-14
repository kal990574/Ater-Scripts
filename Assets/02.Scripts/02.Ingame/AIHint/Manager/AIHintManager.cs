using _02.Scripts.AIHint.Domain;
using _02.Scripts.AIHint.Domain.Models;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _02.Scripts.AIHint.Manager
{
    public class AIHintManager
    {
        private readonly ISpeechToText _stt;
        private readonly ILanguageModel _llm;
        private readonly ITextToSpeech _tts;

        private GameEventPublisher _publisher;
        
        // test
        public ILanguageModel Llm => _llm;
        public ITextToSpeech Tts => _tts;

        public AIHintManager(ISpeechToText stt, ILanguageModel llm, ITextToSpeech tts)
        {
            _stt = stt;
            _llm = llm;
            _tts = tts;
            
            _publisher = new  GameEventPublisher();
            //현재 Mono가 아니어서 인스턴스 아이디가 없는데 안써도 상관은 없음.
            //혹시 Mono와 연결되면 SetSource추가할것
        }

        public async UniTask<HintResult> ProcessHintAsync(
            byte[] audioData,
            PlayerHintState playerState)
        {
            // stt
            string userQuery = await _stt.RecognizeAsync(audioData);
            Debug.Log($"[AIHint] STT 결과: {userQuery}");
            
            // llm
            var request = new HintRequest(userQuery, playerState);
            HintResponse response = await _llm.GenerateHintAsync(request);

            if (!response.IsSuccess)
            {
                Debug.LogWarning($"[AIHint] LLM 실패: {response.HintText}");
                return HintResult.Fail(response.HintText);
            }
            Debug.Log($"[AIHint] 힌트: {response.HintText}");
            //힌트 성공시 이벤트 발행
            _publisher.TryPublish(context => new AiHintAnsweredRawEvent(context), "HintManager");
            
            // tts
            try
            {
                byte[] ttsAudio = await _tts.SynthesizeAsync(response.HintText);
                Debug.Log($"[AIHint] TTS 완료: {ttsAudio.Length} bytes");
                return new HintResult(response.HintText, ttsAudio, true);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[AIHint] TTS 실패, 텍스트만 반환: {e.Message}");
                return new HintResult(response.HintText, null, true);
            }
        }
    }
}