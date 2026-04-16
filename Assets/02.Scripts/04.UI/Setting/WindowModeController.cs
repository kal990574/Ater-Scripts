using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowModeController : MonoBehaviour
{
    private const string PrefKey = "WindowMode";

    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private TextMeshProUGUI _modeLabel;

    private static readonly string[] ModeNames = { "WINDOWED", "FULLSCREEN", "BORDERLESS" };

    private int _currentIndex;

    private void Awake()
    {
        _prevButton.onClick.AddListener(OnPrev);
        _nextButton.onClick.AddListener(OnNext);
    }

    private void Start()
    {
        _currentIndex = PlayerPrefs.GetInt(PrefKey, 1);
        Apply(save: false);
    }
    private void OnDestroy()
    {
        _prevButton.onClick.RemoveListener(OnPrev);
        _nextButton.onClick.RemoveListener(OnNext);
    }

    private void OnPrev()
    {
        _currentIndex--;
        if (_currentIndex < 0)
            _currentIndex = ModeNames.Length - 1;
        Apply(save: true);
    }

    private void OnNext()
    {
        _currentIndex++;
        if (_currentIndex >= ModeNames.Length)
            _currentIndex = 0;
        Apply(save: true);
    }

    private void Apply(bool save)
    {
        _modeLabel.text = ModeNames[_currentIndex];

        switch (_currentIndex)
        {
            case 0: 
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 1: 
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 2: 
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
        }

        if (save)
            PlayerPrefs.SetInt(PrefKey, _currentIndex);
    }
}
