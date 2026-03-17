using UnityEngine;

public class ScannableShaderModifier : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    
    private MaterialPropertyBlock _propertyBlock;

    
    //프로퍼티 조작 요소 추가
    private static readonly int _outlineColorID = Shader.PropertyToID("_OutlineColor");
    private static readonly int _textureBlendingCutoffID = Shader.PropertyToID("_TextureBlendingCutoff");

    private MaterialPropertyBlock _mpb;

    private void Awake()
    {
        _mpb = new MaterialPropertyBlock();
    }

    public void SetOutlineColor(Color outlineColor)
    {
        //추후 두트윈 추가
        _propertyBlock.SetColor(_outlineColorID, outlineColor);
        targetRenderer.SetPropertyBlock(_mpb);
    }

    public void SetBlendCutOff(float cutoff)
    {
        _mpb.SetFloat(_textureBlendingCutoffID, cutoff);
        targetRenderer.SetPropertyBlock(_mpb);
    }


}
