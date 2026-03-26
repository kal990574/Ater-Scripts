using UnityEngine;

public class SoundTest : MonoBehaviour
{
    [SerializeField] private AudioClip testClip;
    private ISoundService _sound;

    private void Start()
    {
        _sound = FindObjectOfType<SoundManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            _sound.PlaySFX(testClip, transform.position);

        if (Input.GetKeyDown(KeyCode.F))
            _sound.PlaySFX2D(testClip);
    }
}
