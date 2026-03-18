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
        
        public string ClientId => _clientId;
        public string ClientSecret => _clientSecret;
        public string SttLanguage => _sttLanguage;
    }
}