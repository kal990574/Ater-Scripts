using UnityEngine;

namespace _02.Scripts.AIHint.Infrastructure.Naver
{
    [CreateAssetMenu(fileName = "NaverCloudConfig", menuName = "Ater/AIHint/NaverCloudConfig")]
    public class NaverCloudConfig : ScriptableObject
    {
        [Header("NCP 인증")] 
        [SerializeField] private string _clientId;
        [SerializeField] private string _clientSecret;

        [Header("CSR 설정")] 
        [SerializeField] private string _sttLanguage = "Kor";

        [Header("Clova Voice 설정")] 
        [SerializeField] private string _ttsSpeaker = "vdaeseong";
        [SerializeField] [Range(-5, 5)] private int _ttsSpeed = -3;
        [SerializeField] [Range(-5, 5)] private int _ttsPitch = -2;
        [SerializeField] [Range(-5, 5)] private int _ttsVolume = 0;
        [SerializeField] [Range(0, 3)] private int _ttsEmotion = 1;
        [SerializeField] [Range(0, 2)] private int _ttsEmotionStrength = 2;
        [SerializeField] [Range(-5, 5)] private int _ttsAlpha = 0;
        [SerializeField] [Range(-5, 5)] private int _ttsEndPitch = -1;
        
        public string ClientId => _clientId;
        public string ClientSecret => _clientSecret;
        public string SttLanguage => _sttLanguage;
        public string TtsSpeaker => _ttsSpeaker;
        public int TtsSpeed => _ttsSpeed;
        public int TtsPitch => _ttsPitch;
        public int TtsVolume => _ttsVolume;
        public int TtsEmotion => _ttsEmotion;
        public int TtsEmotionStrength => _ttsEmotionStrength;
        public int TtsAlpha => _ttsAlpha;
        public int TtsEndPitch => _ttsEndPitch;
    }
}