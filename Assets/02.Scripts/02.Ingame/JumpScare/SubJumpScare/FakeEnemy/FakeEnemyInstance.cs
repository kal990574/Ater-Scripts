using UnityEngine;

public class FakeEnemyInstance : MonoBehaviour
{
    [Header("Runtime")]
    [SerializeField] private float _remainingLifetime;
    [SerializeField] private bool _isInitialized;

    public void Initialize(float lifetime)
    {
        _remainingLifetime = Mathf.Max(0.0f, lifetime);
        _isInitialized = true;
    }

    private void Update()
    {
        if (_isInitialized == false)
        {
            return;
        }

        _remainingLifetime -= Time.deltaTime;

        if (_remainingLifetime > 0.0f)
        {
            return;
        }

        Destroy(gameObject);
    }
}