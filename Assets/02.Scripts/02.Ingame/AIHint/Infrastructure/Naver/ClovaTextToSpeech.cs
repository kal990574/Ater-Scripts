using _02.Scripts.AIHint.Domain;
using Cysharp.Threading.Tasks;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace _02.Scripts.AIHint.Infrastructure.Naver
{
    public class ClovaTextToSpeech : ITextToSpeech
    {
        private readonly NaverCloudConfig _config;
        private const string ApiUrl = "https://naveropenapi.apigw.ntruss.com/tts-premium/v1/tts";
        
        public ClovaTextToSpeech(NaverCloudConfig config)
        {
            _config = config;
        }

        public async UniTask<byte[]> SynthesizeAsync(string text)
        {
            string body = BuildRequestBody(text);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(body);
            
            var request = new UnityWebRequest(ApiUrl, UnityWebRequest.kHttpVerbPOST);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            request.SetRequestHeader("X-NCP-APIGW-API-KEY-ID", _config.ClientId);
            request.SetRequestHeader("X-NCP-APIGW-API-KEY", _config.ClientSecret);

            try
            {
                await request.SendWebRequest().ToUniTask();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception($"[TTS] API 실패 : {request.responseCode} - {request.error}");
                }

                byte[] audioData = request.downloadHandler.data;

                if (audioData == null || audioData.Length == 0)
                {
                    throw new Exception("[TTS] 응답 오디오 데이터가 비어있습니다.");
                }

                return audioData;
            }
            finally
            {
                request.Dispose();
            }
        }

        private string BuildRequestBody(string text)
        {
            var sb = new StringBuilder();
            sb.Append($"speaker={_config.TtsSpeaker}");
            sb.Append($"&text={UnityWebRequest.EscapeURL(text)}");
            sb.Append($"&volume={_config.TtsVolume}");
            sb.Append($"&speed={_config.TtsSpeed}");
            sb.Append($"&pitch={_config.TtsPitch}");
            sb.Append($"&emotion={_config.TtsEmotion}");
            sb.Append($"&emotion-strength={_config.TtsEmotionStrength}");
            sb.Append($"&alpha={_config.TtsAlpha}");
            sb.Append($"&end-pitch={_config.TtsEndPitch}");
            sb.Append("&format=wav");
            
            return sb.ToString();
        }
    }
}