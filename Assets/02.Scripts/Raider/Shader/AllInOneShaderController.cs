using System;
using System.Collections.Generic;
using UnityEngine;

public class AllInOneShaderController : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;

    private MaterialPropertyBlock materialPropertyBlock;
    private readonly Dictionary<string, int> propertyIdCache = new Dictionary<string, int>();

    public void Init()
    {
        if (materialPropertyBlock == null)
        {
            materialPropertyBlock = new MaterialPropertyBlock();
        }

        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }
    }

    private int GetPropertyId(string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException($"[AllInOneController]Property name {propertyName} is not Validate.");
        }

        if (propertyIdCache.TryGetValue(propertyName, out int propertyId))
        {
            return propertyId;
        }
        
        propertyId = Shader.PropertyToID(propertyName);
        propertyIdCache.Add(propertyName, propertyId);
        return propertyId;
    }

    public void SetFloat(string propertyName, float value)
    {
        int propertyId = GetPropertyId(propertyName);
        targetRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetFloat(propertyId, value);
        targetRenderer.SetPropertyBlock(materialPropertyBlock);
    }

    public void SetColor(string propertyName, Color value)
    {
        int propertyId = GetPropertyId(propertyName);
        targetRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetColor(propertyId, value);
        targetRenderer.SetPropertyBlock(materialPropertyBlock);
    }

    public void SetVector(string propertyName, Vector4 value)
    {
        int propertyId = GetPropertyId(propertyName);
        targetRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetVector(propertyId, value);
        targetRenderer.SetPropertyBlock(materialPropertyBlock);
    }
    
}