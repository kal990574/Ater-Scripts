using _02.Scripts.AIHint.Domain;
using _02.Scripts.AIHint.Domain.Models;
using _02.Scripts.AIHint.Infrastructure.OpenAI.DTOs;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace _02.Scripts.AIHint.Infrastructure.OpenAI
{
  public class GPTLanguageModel : ILanguageModel
  {
      private readonly OpenAIConfig _config;
      private readonly string _systemPrompt;
      private const string ApiUrl = "https://api.openai.com/v1/chat/completions";

      public GPTLanguageModel(OpenAIConfig config, string systemPrompt)
      {
          _config = config;
          _systemPrompt = systemPrompt;
      }

      public async UniTask<HintResponse> GenerateHintAsync(HintRequest request)
      {
          string userMessage = BuildUserMessage(request);

          var messages = new List<ChatMessage>
          {
              new ChatMessage("system", _systemPrompt),
              new ChatMessage("user", userMessage)
          };

          var requestBody = new ChatCompletionRequest
          {
              model = _config.Model,
              messages = messages,
              max_completion_tokens = _config.MaxTokens,
              temperature = _config.Temperature
          };

          string jsonBody = JsonUtility.ToJson(requestBody);

          var webRequest = new UnityWebRequest(ApiUrl, UnityWebRequest.kHttpVerbPOST);
          byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

          webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
          webRequest.downloadHandler = new DownloadHandlerBuffer();
          webRequest.SetRequestHeader("Content-Type", "application/json");
          webRequest.SetRequestHeader("Authorization", $"Bearer {_config.ApiKey}");

          try
          {
              await webRequest.SendWebRequest().ToUniTask();

              if (webRequest.result != UnityWebRequest.Result.Success)
              {
                  Debug.LogError($"[LLM] API 실패: {webRequest.responseCode} - {webRequest.error}");
                  return HintResponse.Fail($"API 오류: {webRequest.responseCode}");
              }

              string responseJson = webRequest.downloadHandler.text;
              var response = JsonUtility.FromJson<ChatCompletionResponse>(responseJson);

              if (response.choices == null || response.choices.Count == 0)
              {
                  return HintResponse.Fail("응답이 비어있습니다.");
              }

              string hintText = response.choices[0].message.content;
              return new HintResponse(hintText, true);
          }
          catch (Exception e)
          {
              Debug.LogError($"[LLM] 예외 발생: {e.Message}");
              return HintResponse.Fail(e.Message);
          }
          finally
          {
              webRequest.Dispose();
          }
      }

      private string BuildUserMessage(HintRequest request)
      {
          var sb = new StringBuilder();
          sb.AppendLine("## 플레이어 상태");
          sb.AppendLine($"- 현재 챕터: {request.PlayerState.CurrentChapter}");
          sb.AppendLine($"- 인벤토리: [{string.Join(", ", request.PlayerState.Inventory)}]");
          sb.AppendLine($"- 해결한 퍼즐: [{string.Join(", ", request.PlayerState.SolvedPuzzles)}]");
          sb.AppendLine();
          sb.AppendLine("## 플레이어 질문");
          sb.AppendLine(request.UserQuery);

          return sb.ToString();
      }
  }
}