using System;
using System.Collections.Generic;

namespace _02.Scripts.AIHint.Infrastructure.OpenAI.DTOs
{
    // api spec data 양식 맞춤 -> 코딩 컨벤션 x
    [Serializable]
    public class ChatCompletionRequest
    {
        public string model;
        public List<ChatMessage> messages;
        public int max_completion_tokens;
        public float temperature;
    }

    [Serializable]
    public class ChatMessage
    {
        public string role;
        public string content;

        public ChatMessage(string role, string content)
        {
            this.role = role;
            this.content = content;
        }
    }
}