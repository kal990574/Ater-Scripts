using _02.Scripts.AIHint.Domain;
using _02.Scripts.AIHint.Manager;
using _02.Scripts.AIHint.Domain.Models;
using _02.Scripts.Core;
using _02.Scripts.Core.Domain;
using _02.Scripts.AIHint.Infrastructure.Naver;
using _02.Scripts.AIHint.Infrastructure.OpenAI;
using _02.Scripts.Core.Infrastructure;
using _02.Scripts.Player;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace _02.Scripts.AIHint.Component
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

        [Header("UI")]
        [SerializeField] private HintUIComponent _hintUI;
        
        [Header("phone")]
        [SerializeField] private PhoneViewController _phoneView;

#if UNITY_EDITOR
        [Header("테스트")]
        [SerializeField] private string _testQuery = "금고 비밀번호가 뭐야?";
#endif

        private AIHintManager _hintService;
        private IGameStateProvider _gameStateProvider;
        private IGameManager _gameManager;
        private AudioClip _recordingClip;
        private bool _isRecording;
        private bool _isProcessing;
        private float _recordStartTime;

        private void Start()
        {
            _gameManager = Managers.Get<IGameManager>();
            _gameStateProvider = new GameStateProviderAdapter(_gameManager);
            _gameManager.OnGameStateChanged += OnGameStateChanged;

            var stt = new ClovaSpeechToText(_naverConfig);

            var promptBuilder = new PromptBuilder();
            var chapterData = LoadCurrentChapterData();
            string systemPrompt = promptBuilder.BuildSystemPrompt(chapterData);

            var llm = new GPTLanguageModel(_openAIConfig, systemPrompt);
            var tts = new ClovaTextToSpeech(_naverConfig);

            _hintService = new AIHintManager(stt, llm, tts);
        }

        private void OnDestroy()
        {
            if (_gameManager != null)
            {
                _gameManager.OnGameStateChanged -= OnGameStateChanged;
            }
        }

        private void Update()
        {
            if (_isProcessing) return;
            if (_gameManager.CurrentState != GameState.Playing) return;

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.T))
            {
                TestHintWithText().Forget();
                return;
            }
#endif
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

        private void OnGameStateChanged(GameState state)
        {
            if (state == GameState.Playing) return;

            if (_isRecording)
            {
                Microphone.End(null);
                _isRecording = false;
                _hintUI.Hide();
                _phoneView.Hide().Forget();
            }
        }

        private void StartRecording()
        {
            if (_isRecording) return;
            _recordingClip = Microphone.Start(null, false, _maxRecordSeconds, _sampleRate);

            if (_recordingClip == null)
            {
                Debug.LogWarning("[AIHint] 마이크 시작 실패. 마이크 권한을 확인하세요.");
                return;
            }

            _isRecording = true;
            _recordStartTime = Time.time;
            _phoneView.Show().Forget();
            _hintUI.ShowRecording();
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
                _hintUI.Hide();
                _phoneView.Hide().Forget();
                return;
            }

            var samples = new float[lastPosition];
            _recordingClip.GetData(samples, 0);
            byte[] wavData = WavEncoder.Encode(samples, _sampleRate);

            _hintUI.ShowProcessing();

            try
            {
                var playerState = CollectPlayerState();
                Debug.Log($"[AIHint] 인벤토리: [{string.Join(", ", playerState.Inventory)}]");
                Debug.Log($"[AIHint] 완료작업: [{string.Join(", ", playerState.CompletedTasks)}]");
                HintResult result = await _hintService.ProcessHintAsync(wavData, playerState);
                HandleHintResult(result);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[AIHint] 처리 실패: {e.Message}");
                ShowErrorAndHide("...수신 실패.").Forget();
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
            _phoneView.Show().Forget();
            _hintUI.ShowRecording();
            Debug.Log($"[AIHint-Test] 질문: {_testQuery}");

            try
            {
                _hintUI.ShowProcessing();
                var playerState = CollectPlayerState();
                var request = new HintRequest(_testQuery, playerState);
                HintResponse response = await _hintService.Llm.GenerateHintAsync(request);

                if (!response.IsSuccess)
                {
                    Debug.LogWarning($"[AIHint-Test] LLM 실패: {response.HintText}");
                    _hintUI.Hide();
                    await _phoneView.Hide();
                    return;
                }

                Debug.Log($"[AIHint-Test] 힌트: {response.HintText}");
                _hintUI.ShowResponse(response.HintText);

                byte[] ttsAudio = await _hintService.Tts.SynthesizeAsync(response.HintText);
                Debug.Log($"[AIHint-Test] TTS 완료: {ttsAudio.Length} bytes");

                PlayHintAudio(ttsAudio);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[AIHint-Test] 예외: {e}");
                _hintUI.Hide();
                await _phoneView.Hide();
            }
            finally
            {
                _isProcessing = false;
            }
        }
#endif

        private async void HandleHintResult(HintResult result)
        {
            if (!result.IsSuccess)
            {
                Debug.LogWarning($"[AIHint] 실패: {result.HintText}");
                ShowErrorAndHide("...응답 없음.").Forget();
                return;
            }

            Debug.Log($"[AIHint] 힌트: {result.HintText}");
            _hintUI.ShowResponse(result.HintText);

            if (result.AudioData != null && result.AudioData.Length > 0)
            {
                PlayHintAudio(result.AudioData);
            }
            else
            {
                await _hintUI.HideAfterDelay(0f);
                await _phoneView.Hide();
            }
        }

        private async void PlayHintAudio(byte[] wavData)
        {
            var clip = WavDecoder.Decode(wavData);
            _audioSource.PlayOneShot(clip);
            Debug.Log($"[AIHint] 음성 재생 시작({clip.length:F1}초");
            await _hintUI.HideAfterDelay(clip.length);
            await _phoneView.Hide();
        }

        private async UniTaskVoid ShowErrorAndHide(string message)
        {
            _hintUI.ShowResponse(message);
            await _hintUI.HideAfterDelay(0f);
            await _phoneView.Hide();
        }

        private PlayerHintState CollectPlayerState()
        {
            return new PlayerHintState(
                _gameStateProvider.CurrentChapter,
                _gameStateProvider.GetInventory(),
                _gameStateProvider.GetCompletedTasks());
        }

        private ChapterData LoadCurrentChapterData()
        {
            int chapter = _gameStateProvider.CurrentChapter;
            string fileName = chapter == 0 ? "chapter_tutorial" : $"chapter_{chapter}";
            var json = Resources.Load<TextAsset>($"ChapterData/{fileName}");

            if (json == null)
            {
                Debug.LogWarning($"[AIHint] ChapterData JSON 로드 실패: Resources/ChapterData/{fileName}");
                return new ChapterData();
            }

            return JsonUtility.FromJson<ChapterData>(json.text);
        }
    }
}