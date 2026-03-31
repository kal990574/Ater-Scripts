using _02.Scripts.AIHint.Domain;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace _02.Scripts.AIHint.Infrastructure.Naver
{
    public class ClovaSpeechToText : ISpeechToText
    {
        private readonly NaverCloudConfig _config;
        private const string BaseUrl = "https://naveropenapi.apigw.ntruss.com/recog/v1/stt";

        public ClovaSpeechToText(NaverCloudConfig config)
        {
            _config = config;
        }

        public async UniTask<string> RecognizeAsync(byte[] audioData)
        {
            string url = $"{BaseUrl}?lang={_config.SttLanguage}";
            var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            
            var uploadHandler = new UploadHandlerRaw(audioData);
            uploadHandler.contentType = "application/octet-stream";
            request.uploadHandler = uploadHandler;
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("X-NCP-APIGW-API-KEY-ID", _config.ClientId);
            request.SetRequestHeader("X-NCP-APIGW-API-KEY", _config.ClientSecret);
            
            await request.SendWebRequest().ToUniTask();

            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"[CLOVA CSR] {request.responseCode}: {request.error}");
            }
            
            string json = request.downloadHandler.text;
            var response = JsonUtility.FromJson<ClovaSttResponse>(json);
            
            if (string.IsNullOrEmpty(response.text))
            {
                throw new Exception("[CLOVA CSR] 인식된 텍스트가 비어있습니다.");
            }

            return response.text;
        }

        [Serializable]
        private struct ClovaSttResponse
        {
            public string text;
        }
    }
}