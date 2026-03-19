using _02.Scripts.AIHint.Application.Services;
using _02.Scripts.AIHint.Domain;
using _02.Scripts.AIHint.Domain.Models;
using _02.Scripts.AIHint.Infrastructure;
using _02.Scripts.AIHint.Infrastructure.Naver;
using _02.Scripts.AIHint.Infrastructure.OpenAI;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _02.Scripts.AIHint.Presentation
{
    public class HintInputController : MonoBehaviour
    {
        [Header("설정")]
        [SerializeField] private NaverCloudConfig _naverConfig;
        [SerializeField] private OpenAIConfig _openAIConfig;
        [SerializeField] private KeyCode _pttKey = KeyCode.R;

        [Header("녹음 설정")]
        [SerializeField] private int _maxRecordSeconds = 10;
        [SerializeField] private int _sampleRate = 16000;

        [Header("테스트")]
        [SerializeField] private KeyCode _testKey = KeyCode.T;
        [SerializeField] private string _testQuery = "금고 비밀번호가 뭐야?";

        private AIHintService _hintService;
        private IGameStateProvider _gameStateProvider;
        private AudioClip _recordingClip;
        private bool _isRecording;
        private bool _isProcessing;

        private void Start()
        {
            _gameStateProvider = new DummyGameStateProvider();

            var stt = new ClovaSpeechToText(_naverConfig);

            var promptBuilder = new PromptBuilder();
            var chapterData = LoadCurrentChapterData();
            string systemPrompt = promptBuilder.BuildSystemPrompt(chapterData);

            var llm = new GPTLanguageModel(_openAIConfig, systemPrompt);

            _hintService = new AIHintService(stt, llm);
        }

        private void Update()
        {
            if (_isProcessing) return;

            if (Input.GetKeyDown(_testKey))
            {
                TestHintWithText().Forget();
                return;
            }

            if (Input.GetKeyDown(_pttKey))
            {
                StartRecording();
            }
            else if (Input.GetKeyUp(_pttKey) && _isRecording)
            {
                StopRecordingAndRecognize().Forget();
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
                HintResponse response = await _hintService.ProcessHintAsync(wavData, playerState);

                if (response.IsSuccess)
                {
                    Debug.Log($"[AIHint] 힌트: {response.HintText}");
                }
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

        private async UniTaskVoid TestHintWithText()
        {
            _isProcessing = true;
            Debug.Log($"[AIHint-Test] 질문: {_testQuery}");

            try
            {
                var playerState = CollectPlayerState();
                var request = new HintRequest(_testQuery, playerState);
                HintResponse response = await _hintService.Llm.GenerateHintAsync(request);

                if (response.IsSuccess)
                {
                    Debug.Log($"[AIHint-Test] 힌트: {response.HintText}");
                }
                else
                {
                    Debug.LogWarning($"[AIHint-Test] 실패: {response.HintText}");
                }
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

        private PlayerHintState CollectPlayerState()
        {
            return new PlayerHintState(
                _gameStateProvider.CurrentChapter,
                _gameStateProvider.CurrentRoom,
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