using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _02.Scripts._02.Ingame.EndingSequence
{
    public class GlitchOverlayUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RawImage _scanlineImage;
        [SerializeField] private RawImage _noiseImage;

        [Header("Texture Generation")]
        [SerializeField] private int _textureWidth = 960;
        [SerializeField] private int _textureHeight = 540;

        [Header("Scanline")]
        [SerializeField] private float _scanlineAlpha = 0.15f;

        [Header("Noise Timing")]
        [SerializeField] private float _minInterval = 0.05f;
        [SerializeField] private float _maxInterval = 0.3f;
        [SerializeField] private float _minDuration = 0.03f;
        [SerializeField] private float _maxDuration = 0.15f;

        [Header("Noise Intensity")]
        [SerializeField] private float _noiseMaxAlpha = 0.4f;

        private Coroutine _glitchRoutine;
        private Texture2D[] _generatedTextures;

        private void Awake()
        {
            GenerateTextures();
        }

        public void StartGlitch()
        {
            _canvasGroup.alpha = 1f;

            // 스캔라인 상시 표시
            _scanlineImage.gameObject.SetActive(true);
            _scanlineImage.color = new Color(1f, 1f, 1f, _scanlineAlpha);

            // 노이즈 깜빡임 시작
            _noiseImage.gameObject.SetActive(false);
            _glitchRoutine = StartCoroutine(NoiseFlickerLoop());
        }

        public void StopGlitch()
        {
            if (_glitchRoutine != null) StopCoroutine(_glitchRoutine);
            HideAll();
        }

        public void SetIntensity(float t)
        {
            // 스캔라인 알파도 같이 올라감
            _scanlineAlpha = Mathf.Lerp(0.1f, 0.35f, t);
            _scanlineImage.color = new Color(1f, 1f, 1f, _scanlineAlpha);

            // 노이즈 빈도 + 강도
            _noiseMaxAlpha = Mathf.Lerp(0.15f, 0.5f, t);
            _maxInterval = Mathf.Lerp(0.3f, 0.03f, t);
        }

        private void GenerateTextures()
        {
            _generatedTextures = new Texture2D[2];
            _generatedTextures[0] = GenerateScanlineTexture();
            _generatedTextures[1] = GenerateNoiseTexture();

            _scanlineImage.texture = _generatedTextures[0];
            _noiseImage.texture = _generatedTextures[1];
        }

        // 아날로그 스캔라인
        private Texture2D GenerateScanlineTexture()
        {
            var tex = new Texture2D(_textureWidth, _textureHeight, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;

            for (int y = 0; y < _textureHeight; y++)
            {
                bool isScanline = y % 6 < 3;
                for (int x = 0; x < _textureWidth; x++)
                {
                    tex.SetPixel(x, y, isScanline
                        ? new Color(0f, 0f, 0f, 0.8f)
                        : Color.clear);
                }
            }

            tex.Apply();
            return tex;
        }

        // 고주파 디지털 노이즈
        private Texture2D GenerateNoiseTexture()
        {
            var tex = new Texture2D(_textureWidth, _textureHeight, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;

            for (int y = 0; y < _textureHeight; y++)
            {
                bool isNoiseLine = Random.value > 0.7f;
                for (int x = 0; x < _textureWidth; x++)
                {
                    if (isNoiseLine)
                    {
                        float v = Random.value;
                        tex.SetPixel(x, y, new Color(v, v, v, Random.Range(0.1f, 0.4f)));
                    }
                    else
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        private IEnumerator NoiseFlickerLoop()
        {
            while (true)
            {
                // 대기
                yield return new WaitForSecondsRealtime(
                    Random.Range(_minInterval, _maxInterval));

                // 노이즈 켜기 + UV 흔들기
                _noiseImage.uvRect = new Rect(
                    Random.Range(-0.5f, 0.5f),
                    Random.Range(-0.5f, 0.5f), 1f, 1f);
                _noiseImage.color = new Color(1f, 1f, 1f, Random.Range(0.1f, _noiseMaxAlpha));
                _noiseImage.gameObject.SetActive(true);

                // 유지
                yield return new WaitForSecondsRealtime(
                    Random.Range(_minDuration, _maxDuration));

                // 노이즈 끄기
                _noiseImage.gameObject.SetActive(false);
            }
        }

        private void HideAll()
        {
            _canvasGroup.alpha = 0f;
            _scanlineImage.gameObject.SetActive(false);
            _noiseImage.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_generatedTextures == null) return;
            foreach (var tex in _generatedTextures)
                if (tex != null) Destroy(tex);
        }
    }
}