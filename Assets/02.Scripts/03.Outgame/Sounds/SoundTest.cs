using UnityEngine;


public class SoundTest : MonoBehaviour
{
    [SerializeField] private SoundManager _soundManager;

    private void Start()
    {
        _soundManager.PlayBGM(SoundKey.BGM_Tutorial);
        Debug.Log("[SoundTest] BGM_Chapter1 재생 시작");
    }
}