using _02.Scripts.AIHint.Application.Services;
using _02.Scripts.AIHint.Domain.Models;
using _02.Scripts.Core;
using _02.Scripts.Core.Domain;
using _02.Scripts.AIHint.Infrastructure.Naver;
using _02.Scripts.AIHint.Infrastructure.OpenAI;
using _02.Scripts.Core.Infrastructure;
using _02.Scripts.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _02.Scripts.AIHint.Presentation
{
    public class HintInputController : MonoBehaviour
    {
        [Header("입력")]
        [SerializeField] private PlayerInputHandler _inputHandler;

        [Header("설정")]
        [SerializeField] private NaverCloudConfig _naverConfig;
        [SerializeField] private OpenAIConfig _openAIConfig;

        [Header("녹음 설정")]
        [SerializeField] private int _maxRecordSeconds = 5;
        [SerializeField] private int _sampleRate = 16000;

        [Header("오디오 출력")]
        [SerializeField] private AudioSource _audioSource;

#if UNITY_EDITOR
        [Header("테스트")]
        [SerializeField] private string _testQuery = "금고 비밀번호가 뭐야?";
#endif

        private AIHintService _hintService;
        private IGameStateProvider _gameStateProvider;
        private AudioClip _recordingClip;
        private bool _isRecording;
        private bool _isProcessing;
        private float _recordStartTime;

        private void Start()
        {
            var gameManager = ServiceLocator.Get<IGameManager>();
            _gameStateProvider = new GameStateProviderAdapter(gameManager);

            var stt = new ClovaSpeechToText(_naverConfig);

            var promptBuilder = new PromptBuilder();
            var chapterData = LoadCurrentChapterData();
            string systemPrompt = promptBuilder.BuildSystemPrompt(chapterData);

            var llm = new GPTLanguageModel(_openAIConfig, systemPrompt);
            var tts = new ClovaTextToSpeech(_naverConfig);

            _hintService = new AIHintService(stt, llm, tts);
        }

        private void Update()
        {
            if (_isProcessing) return;

            if (_isRecording && Time.time - _recordStartTime >= _maxRecordSeconds)
            {
                StopRecordingAndRecognize().Forget();
                return;
            }

            if (_inputHandler.HintToggleInput)
            {
                if (_isRecording)
                {
                    StopRecordingAndRecognize().Forget();
                }
                else
                {
                    StartRecording();
                }
            }
        }

        private void StartRecording()
        {
            if (_isRecording) return;
            _recordingClip = Microphone.Start(null, false, _maxRecordSeconds, _sampleRate);

            if (_recordingClip == null)
            {
                Debug.LogError("[AIHint] 마이크 시작 실패. 마이크 권한을 확인하세요.");
                return;
            }

            _isRecording = true;
            _recordStartTime = Time.time;
            Debug.Log("[AIHint] 녹음 시작...");
        }

        private async UniTaskVoid StopRecordingAndRecognize()
        {
            int lastPosition = Microphone.GetPosition(null);
            Microphone.End(null);
            _isRecording = false;
            _isProcessing = true;

            if (lastPosition == 0)
            {
                _isProcessing = false;
                return;
            }

            var samples = new float[lastPosition];
            _recordingClip.GetData(samples, 0);
            byte[] wavData = WavEncoder.Encode(samples, _sampleRate);

            try
            {
                var playerState = CollectPlayerState();
                HintResult result = await _hintService.ProcessHintAsync(wavData, playerState);
                HandleHintResult(result);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AIHint] 처리 실패: {e.Message}");
            }
            finally
            {
                _isProcessing = false;
            }
        }
        
#if UNITY_EDITOR
        private async UniTaskVoid TestHintWithText()
        {
            _isProcessing = true;
            Debug.Log($"[AIHint-Test] 질문: {_testQuery}");

            try
            {
                var playerState = CollectPlayerState();
                var request = new HintRequest(_testQuery, playerState);
                HintResponse response = await _hintService.Llm.GenerateHintAsync(request);

                if (!response.IsSuccess)
                {
                    Debug.LogWarning($"[AIHint-Test] LLM 실패: {response.HintText}");
                    return;
                }

                Debug.Log($"[AIHint-Test] 힌트: {response.HintText}");

                byte[] ttsAudio = await _hintService.Tts.SynthesizeAsync(response.HintText);
                Debug.Log($"[AIHint-Test] TTS 완료: {ttsAudio.Length} bytes");

                PlayHintAudio(ttsAudio);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AIHint-Test] 예외: {e.Message}");
            }
            finally
            {
                _isProcessing = false;
            }
        }
#endif

        private void HandleHintResult(HintResult result)
        {
            if (!result.IsSuccess)
            {
                Debug.LogWarning($"[AIHint] 실패: {result.HintText}");
                return;
            }
            
            Debug.Log($"[AIHint] 힌트: {result.HintText}");

            if (result.AudioData != null && result.AudioData.Length > 0)
            {
                PlayHintAudio(result.AudioData);
            }
        }

        private void PlayHintAudio(byte[] wavData)
        {
            var clip = WavDecoder.Decode(wavData);
            _audioSource.PlayOneShot(clip);
            Debug.Log($"[AIHint] 음성 재생 시작({clip.length:F1}초");
        }

        private PlayerHintState CollectPlayerState()
        {
            return new PlayerHintState(
                _gameStateProvider.CurrentChapter,
                _gameStateProvider.GetInventory(),
                _gameStateProvider.GetSolvedPuzzles());
        }

        private ChapterData LoadCurrentChapterData()
        {
            var json = Resources.Load<TextAsset>("ChapterData/chapter_1");

            if (json == null)
            {
                Debug.LogError("[AIHint] ChapterData JSON 로드 실패: Resources/ChapterData/chapter_1");
                return new ChapterData();
            }

            return JsonUtility.FromJson<ChapterData>(json.text);
        }
    }
}