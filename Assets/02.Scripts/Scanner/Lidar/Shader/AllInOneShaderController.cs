using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Renderer))]
public class AllInOneShaderController : MonoBehaviour
{
    [Header("Required References")]
    [FormerlySerializedAs("targetRenderer")]
    [SerializeField] private Renderer _targetRenderer;

    private MaterialPropertyBlock _materialPropertyBlock;
    private readonly Dictionary<string, int> _propertyIdCache = new();

    public void Init()
    {
        if (_materialPropertyBlock == null)
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
        }

        if (_targetRenderer == null)
        {
            _targetRenderer = GetComponent<Renderer>();
        }
    }

    public void SetFloat(string propertyName, float value)
    {
        int propertyId = GetPropertyId(propertyName);
        ApplyPropertyBlock(block => block.SetFloat(propertyId, value));
    }

    public void SetColor(string propertyName, Color value)
    {
        int propertyId = GetPropertyId(propertyName);
        ApplyPropertyBlock(block => block.SetColor(propertyId, value));
    }

    public void SetVector(string propertyName, Vector4 value)
    {
        int propertyId = GetPropertyId(propertyName);
        ApplyPropertyBlock(block => block.SetVector(propertyId, value));
    }

    private int GetPropertyId(string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException($"[AllInOneController]Property name {propertyName} is not Validate.");
        }

        if (_propertyIdCache.TryGetValue(propertyName, out int propertyId))
        {
            return propertyId;
        }

        propertyId = Shader.PropertyToID(propertyName);
        _propertyIdCache.Add(propertyName, propertyId);
        return propertyId;
    }

    private void ApplyPropertyBlock(Action<MaterialPropertyBlock> apply)
    {
        EnsureInitialized();
        if (_targetRenderer == null)
        {
            throw new InvalidOperationException($"[{nameof(AllInOneShaderController)}] Renderer is missing.");
        }

        _targetRenderer.GetPropertyBlock(_materialPropertyBlock);
        apply(_materialPropertyBlock);
        _targetRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    private void EnsureInitialized()
    {
        if (_materialPropertyBlock == null || _targetRenderer == null)
        {
            Init();
        }
    }
}
