using Cysharp.Threading.Tasks;
using Febucci.TextAnimatorForUnity;
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
        
        [Header("typing")]
        [SerializeField] private TypewriterComponent _typewriter;

        [Header("설정")]
        [SerializeField] private float _hideDelayAfterTTS = 2f;

        public void ShowRecording()
        {
            _modalWindow.ModalWindowIn();
            _titleText.text = "녹음중..";
        }

        public void ShowProcessing()
        {
            _titleText.text = "처리중..";
        }

        public void ShowResponse(string text)
        {
            _titleText.text = "???";
            _typewriter.ShowText(text);
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
