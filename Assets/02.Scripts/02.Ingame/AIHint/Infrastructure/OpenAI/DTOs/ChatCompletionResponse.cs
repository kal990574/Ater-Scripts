using System;
using System.Collections.Generic;

namespace _02.Scripts.AIHint.Infrastructure.OpenAI.DTOs
{
    // api spec data 양식 맞춤 -> 코딩 컨벤션 x
    [Serializable]
    public class ChatCompletionResponse
    {
        public List<Choice> choices;
    }

    [Serializable]
    public class Choice
    {
        public ChatMessageResponse message;
    }

    [Serializable]
    public class ChatMessageResponse
    {
        public string role;
        public string content;
    }
}