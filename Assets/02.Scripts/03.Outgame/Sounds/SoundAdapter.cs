using UnityEngine;

public class SoundAdapter : MonoBehaviour
{
    [SerializeField] private string _soundKey;
    [SerializeField] private Transform _soundPosition;
    private SoundManager _soundManager;

    private void Awake()
    {
        _soundManager = FindFirstObjectByType<SoundManager>();
    }

    public void PlaySFX()
    {
        Vector3 pos = _soundPosition != null ? _soundPosition.position : transform.position;
        _soundManager?.PlaySFX(_soundKey, transform.position);
    }

    public void PlaySFX2D()
    {
        _soundManager?.PlaySFX2D(_soundKey);
    }
}
