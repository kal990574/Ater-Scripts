using UnityEngine;

namespace _02.Scripts.AIHint.Infrastructure.OpenAI
{
    [CreateAssetMenu(fileName = "OpenAIConfig", menuName = "Ater/AIHint/OpenAIConfig")]
    public class OpenAIConfig : ScriptableObject
    {
        [Header("OpenAI 인증")] 
        [SerializeField] private string _apiKey;

        [Header("LLM 설정")] 
        [SerializeField] private string _model = "gpt-5.4-nano";

        [SerializeField] private int _maxTokens = 150;
        [SerializeField] [Range(0f, 2f)] private float _temperature = 0.8f;
        
        public string  ApiKey => _apiKey;
        public string Model => _model;
        public int MaxTokens => _maxTokens;
        public float Temperature => _temperature;
    }
}