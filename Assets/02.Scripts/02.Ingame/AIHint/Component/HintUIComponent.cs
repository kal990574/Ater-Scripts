using Cysharp.Threading.Tasks;
using Michsky.UI.Dark;
using TMPro;
using UnityEngine;

namespace _02.Scripts.AIHint.Component
{
    public class HintUIComponent : MonoBehaviour
    {
        [Header("Modal Window")]
        [SerializeField] private ModalWindowManager _modalWindow;

        [Header("텍스트")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _responseText;

        [Header("설정")]
        [SerializeField] private float _hideDelayAfterTTS = 2f;

        public void ShowRecording()
        {
            _responseText.text = "";
            _titleText.text = "녹음 중...";
            _modalWindow.ModalWindowIn();
        }

        public void ShowProcessing()
        {
            _titleText.text = "...";
        }

        public void ShowResponse(string text)
        {
            _titleText.text = "힌트";
            _responseText.text = text;
        }

        public async UniTask HideAfterDelay(float ttsClipLength)
        {
            float delay = ttsClipLength + _hideDelayAfterTTS;
            await UniTask.Delay((int)(delay * 1000), ignoreTimeScale: true);
            Hide();
        }

        public void Hide()
        {
            _modalWindow.ModalWindowOut();
        }
    }
}
