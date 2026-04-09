using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Renderer))]
public class AllInOneShaderController : MonoBehaviour
{
    public enum OutlineType
    {
        None = 0,
        Simple = 1,
        Constant = 2,
        FadeWithDistance = 3
    }

    private const string OUTLINE_TYPE_PROPERTY_NAME = "_OutlineType";
    private const string OUTLINE_TYPE_NONE_KEYWORD = "_OUTLINETYPE_NONE";
    private const string OUTLINE_TYPE_SIMPLE_KEYWORD = "_OUTLINETYPE_SIMPLE";
    private const string OUTLINE_TYPE_CONSTANT_KEYWORD = "_OUTLINETYPE_CONSTANT";
    private const string OUTLINE_TYPE_FADE_WITH_DISTANCE_KEYWORD = "_OUTLINETYPE_FADEWITHDISTANCE";

    [Header("Required References")]
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

    public void SetOutlineEnabled(bool enabled)
    {
        SetOutlineType(enabled ? OutlineType.Simple : OutlineType.None);
    }

    public void SetOutlineType(OutlineType outlineType)
    {
        EnsureInitialized();
        if (_targetRenderer == null)
        {
            throw new InvalidOperationException($"[{nameof(AllInOneShaderController)}] Renderer is missing.");
        }

        Material[] materials = _targetRenderer.materials;
        float outlineTypeValue = (float)outlineType;
        string enabledKeyword = GetOutlineKeyword(outlineType);

        for (int i = 0; i < materials.Length; i++)
        {
            Material material = materials[i];
            if (material == null || material.HasProperty(OUTLINE_TYPE_PROPERTY_NAME) == false)
            {
                continue;
            }

            material.SetFloat(OUTLINE_TYPE_PROPERTY_NAME, outlineTypeValue);
            material.DisableKeyword(OUTLINE_TYPE_NONE_KEYWORD);
            material.DisableKeyword(OUTLINE_TYPE_SIMPLE_KEYWORD);
            material.DisableKeyword(OUTLINE_TYPE_CONSTANT_KEYWORD);
            material.DisableKeyword(OUTLINE_TYPE_FADE_WITH_DISTANCE_KEYWORD);
            material.EnableKeyword(enabledKeyword);
        }
    }

    private static string GetOutlineKeyword(OutlineType outlineType)
    {
        return outlineType switch
        {
            OutlineType.Simple => OUTLINE_TYPE_SIMPLE_KEYWORD,
            OutlineType.Constant => OUTLINE_TYPE_CONSTANT_KEYWORD,
            OutlineType.FadeWithDistance => OUTLINE_TYPE_FADE_WITH_DISTANCE_KEYWORD,
            _ => OUTLINE_TYPE_NONE_KEYWORD
        };
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
