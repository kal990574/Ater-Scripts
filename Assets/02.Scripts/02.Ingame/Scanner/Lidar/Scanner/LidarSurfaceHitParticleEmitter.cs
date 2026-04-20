using UnityEngine;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class LidarSurfaceHitParticleEmitter : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private ParticleSystem _particleSystem;

    [Header("Emission")]
    [SerializeField] private float _emitInterval = 0.03f;
    [SerializeField] private int _emitCountPerTick = 2;
    [SerializeField] private float _surfaceOffset = 0.01f;
    [SerializeField] private float _outwardSpeedMin = 0.15f;
    [SerializeField] private float _outwardSpeedMax = 0.6f;
    [SerializeField] private float _spreadAngle = 20.0f;

    private float _elapsedSinceEmit;

    private void Awake()
    {
        if (_particleSystem == null)
        {
            _particleSystem = GetComponentInChildren<ParticleSystem>();
        }
    }

    public void ResetEmitter()
    {
        _elapsedSinceEmit = 0.0f;

        if (_particleSystem == null)
        {
            return;
        }

        _particleSystem.Clear();
    }

    public void Emit(LidarSurfaceHitSample hitSample, float deltaTime)
    {
        if (_particleSystem == null)
        {
            return;
        }

        if (_emitCountPerTick <= 0)
        {
            return;
        }

        _elapsedSinceEmit += Mathf.Max(0.0f, deltaTime);
        if (_elapsedSinceEmit < _emitInterval)
        {
            return;
        }

        _elapsedSinceEmit = 0.0f;
        for (int i = 0; i < _emitCountPerTick; i++)
        {
            EmitSingle(hitSample);
        }
    }

    private void EmitSingle(LidarSurfaceHitSample sample)
    {
        Vector3 normal = sample.Normal.sqrMagnitude > 0.0001f ? sample.Normal.normalized : Vector3.up;
        Quaternion spreadRotation = Quaternion.AngleAxis(Random.Range(-_spreadAngle, _spreadAngle), Random.onUnitSphere);
        Vector3 velocity = spreadRotation * normal * Random.Range(_outwardSpeedMin, _outwardSpeedMax);

        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
        {
            position = sample.Point + normal * _surfaceOffset,
            velocity = velocity
        };

        _particleSystem.Emit(emitParams, 1);
    }
}
