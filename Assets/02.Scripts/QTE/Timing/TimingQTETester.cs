using UnityEngine;

public class TimingQTETester : MonoBehaviour
{
    [SerializeField] private TimingQTERunner _skillCheck;
    [SerializeField] private KeyCode _spawnKey = KeyCode.F;

    [Header("Auto Trigger")]
    [SerializeField] private bool _useAutoTrigger = true;
    [SerializeField] private float _interval = 5f;

    private float _timer;

    
    private void Update()
    {
        // 수동 트리거
        if (Input.GetKeyDown(_spawnKey) == true)
        {
            TryStartSkillCheck();
        }

        // 자동 트리거
        if (_useAutoTrigger == true)
        {
            _timer += Time.deltaTime;

            if (_timer >= _interval)
            {
                _timer = 0f;
                TryStartSkillCheck();
            }
        }
    }

    private void TryStartSkillCheck()
    {
        if (_skillCheck == null)
        {
            return;
        }

        if (_skillCheck.IsPlaying == true)
        {
            return;
        }
    }
}