using UnityEngine;

//점프스케어를 발생시키는 실행자

public class JumpScareManager : MonoBehaviour
{
    public void PlayJumpScare(int id)
    {
        Debug.Log($"Play {id} JumpScare");
    }
}