using _02.Scripts.AIHint.Domain;
using _02.Scripts.AIHint.Infrastructure.Naver;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _02.Scripts.AIHint.Presentation
{
    public class HintInputController : MonoBehaviour
    {
        [Header("설정")] 
        [SerializeField] private NaverCloudConfig _config;
        [SerializeField] private KeyCode _pttKey = KeyCode.R;

        [Header("녹음 설정")] 
        [SerializeField] private int _maxRecordSeconds = 10;
        [SerializeField] private int _sampleRate = 16000;

        private ISpeechToText _stt;
        private AudioClip _recordingClip;
        private bool _isRecording;
        private bool _isProcessing;

        private void Start()
        {
            _stt = new ClovaSpeechToText(_config);
        }

        private void Update()
        {
            if (_isProcessing) return;
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

            Debug.Log($"[STT] 마이크 장치 수: {Microphone.devices.Length}");
            foreach (string device in Microphone.devices)
            {
                Debug.Log($"[STT] 마이크: {device}");
            }

            _recordingClip = Microphone.Start(null, false, _maxRecordSeconds, _sampleRate);

            if (_recordingClip == null)
            {
                Debug.LogError("[STT] 마이크 시작 실패. 마이크 권한을 확인하세요.");
                return;
            }

            _isRecording = true;
            Debug.Log($"[STT] 녹음 시작... clip: {_recordingClip.frequency}Hz, {_recordingClip.channels}ch, {_recordingClip.samples}samples");
        }

        private async UniTaskVoid StopRecordingAndRecognize()
        {
            int lastPosition = Microphone.GetPosition(null);
            bool isStillRecording = Microphone.IsRecording(null);
            Microphone.End(null);
            _isRecording = false;
            _isProcessing = true;

            Debug.Log($"[STT] 녹음 종료. position: {lastPosition}, isRecording: {isStillRecording}, clip null: {_recordingClip == null}");

            if (lastPosition == 0)
            {
                _isProcessing = false;
                return;
            }
            
            var samples = new float[lastPosition];
            _recordingClip.GetData(samples, 0);
            
            byte[] wavData = WavEncoder.Encode(samples, _sampleRate);
            Debug.Log($"[STT] WAV 변환 완료.. 크기: {wavData.Length}bytes");

            try
            {
                string result = await _stt.RecognizeAsync(wavData);
                Debug.Log($"[STT] 변환 결과: {result}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[STT] 인식 실패: {e.Message}");
            }
            finally
            {
                _isProcessing = false;
            }
        }
    }
}