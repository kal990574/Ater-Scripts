using UnityEngine;

public class MinigameExample : MonoBehaviour, IScanMinigame
{
    [Header("Input")]
    [SerializeField] private KeyCode _submitKey = KeyCode.E;

    [Header("Timing")]
    [SerializeField] private float _warningDelay = 0.45f;
    [SerializeField] private float _rotationSpeed = 270.0f;
    [SerializeField] private float _autoFailTime = 2.5f;

    [Header("Zone")]
    [SerializeField] private Vector2 _successZoneSizeRange = new Vector2(50.0f, 90.0f);
    [SerializeField] private Vector2 _greatZoneSizeRange = new Vector2(8.0f, 18.0f);
    [SerializeField] private float _greatZoneOffsetInsideSuccess = 0.0f;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _warningClip;
    [SerializeField] private AudioClip _successClip;
    [SerializeField] private AudioClip _greatSuccessClip;
    [SerializeField] private AudioClip _failClip;

    [Header("UI")]
    [SerializeField] private MinigameUI _ui;
    
    private bool _isPlaying;
    private bool _isFinished;
    private bool _spinnerVisible;
    private float _elapsedTime;
    private float _spinnerElapsedTime;
    private float _needleAngle;
    private float _successStartAngle;
    private float _successEndAngle;
    private float _greatStartAngle;
    private float _greatEndAngle;
    private MinigameResult _result = MinigameResult.Default;

    public bool IsPlaying => _isPlaying;
    public bool IsFinished => _isFinished;
    public MinigameResult Result => _result;

    public float NeedleAngle => _needleAngle;
    public float SuccessStartAngle => _successStartAngle;
    public float SuccessEndAngle => _successEndAngle;
    public float GreatStartAngle => _greatStartAngle;
    public float GreatEndAngle => _greatEndAngle;
    public bool SpinnerVisible => _spinnerVisible;
    
    public void Begin()
    {
        _isPlaying = true;
        _isFinished = false;
        _spinnerVisible = false;
        _elapsedTime = 0.0f;
        _spinnerElapsedTime = 0.0f;
        _needleAngle = 0.0f;
        _result = MinigameResult.Default;

        GenerateZones();

        if (_audioSource != null && _warningClip != null)
        {
            _audioSource.PlayOneShot(_warningClip);
        }

        if (_ui != null)
        {
            _ui.Hide();
        }
    }

    public void Tick(float deltaTime)
    {
        if (_isPlaying == false || _isFinished == true)
        {
            return;
        }

        _elapsedTime += deltaTime;

        if (_spinnerVisible == false)
        {
            if (_elapsedTime >= _warningDelay)
            {
                ShowSpinner();
            }

            return;
        }

        _spinnerElapsedTime += deltaTime;
        _needleAngle = NormalizeAngle(_needleAngle + (_rotationSpeed * deltaTime));

        if (_ui != null)
        {
            _ui.SetNeedleAngle(_needleAngle);
        }

        if (Input.GetKeyDown(_submitKey) == true)
        {
            Submit();
            return;
        }

        if (_spinnerElapsedTime >= _autoFailTime)
        {
            SetResult(MinigameResult.Fail);
        }
    }

    public void Submit()
    {
        if (_isPlaying == false || _isFinished == true)
        {
            return;
        }

        if (_spinnerVisible == false)
        {
            SetResult(MinigameResult.Fail);
            return;
        }

        if (IsAngleInsideZone(_needleAngle, _greatStartAngle, _greatEndAngle) == true)
        {
            SetResult(MinigameResult.GreatSuccess);
            return;
        }

        if (IsAngleInsideZone(_needleAngle, _successStartAngle, _successEndAngle) == true)
        {
            SetResult(MinigameResult.Success);
            return;
        }

        SetResult(MinigameResult.Fail);
    }

    public void Cancel()
    {
        if (_isPlaying == false)
        {
            return;
        }

        _isPlaying = false;
        _isFinished = true;
        _result = MinigameResult.Fail;

        if (_ui != null)
        {
            _ui.Hide();
        }
    }

    private void ShowSpinner()
    {
        _spinnerVisible = true;
        _spinnerElapsedTime = 0.0f;
        _needleAngle = 0.0f;

        if (_ui != null)
        {
            _ui.Show();
            _ui.SetZones(_successStartAngle, _successEndAngle, _greatStartAngle, _greatEndAngle);
            _ui.SetNeedleAngle(_needleAngle);
        }
    }

    private void SetResult(MinigameResult result)
    {
        _result = result;
        _isFinished = true;
        _isPlaying = false;

        if (_audioSource != null)
        {
            switch (_result)
            {
                case MinigameResult.Success:
                {
                    if (_successClip != null)
                    {
                        _audioSource.PlayOneShot(_successClip);
                    }

                    break;
                }
                case MinigameResult.GreatSuccess:
                {
                    if (_greatSuccessClip != null)
                    {
                        _audioSource.PlayOneShot(_greatSuccessClip);
                    }

                    break;
                }
                case MinigameResult.Fail:
                {
                    if (_failClip != null)
                    {
                        _audioSource.PlayOneShot(_failClip);
                    }

                    break;
                }
            }
        }

        // if (_ui != null)
        // {
        //     _ui.Hide();
        // }
    }

    private void GenerateZones()
    {
        float successSize = Random.Range(_successZoneSizeRange.x, _successZoneSizeRange.y);
        float greatSize = Random.Range(_greatZoneSizeRange.x, _greatZoneSizeRange.y);
        greatSize = Mathf.Min(greatSize, successSize);

        float successCenter = Random.Range(0.0f, 360.0f);

        _successStartAngle = NormalizeAngle(successCenter - (successSize * 0.5f));
        _successEndAngle = NormalizeAngle(successCenter + (successSize * 0.5f));

        float greatCenter = NormalizeAngle(successCenter + _greatZoneOffsetInsideSuccess);
        float maxGreatHalfSize = successSize * 0.5f;
        float requestedGreatHalfSize = greatSize * 0.5f;
        float actualGreatHalfSize = Mathf.Min(requestedGreatHalfSize, maxGreatHalfSize);

        _greatStartAngle = NormalizeAngle(greatCenter - actualGreatHalfSize);
        _greatEndAngle = NormalizeAngle(greatCenter + actualGreatHalfSize);
    }

    private bool IsAngleInsideZone(float angle, float start, float end)
    {
        angle = NormalizeAngle(angle);
        start = NormalizeAngle(start);
        end = NormalizeAngle(end);

        if (start <= end)
        {
            return angle >= start && angle <= end;
        }

        return angle >= start || angle <= end;
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360.0f;

        if (angle < 0.0f)
        {
            angle += 360.0f;
        }

        return angle;
    }
}