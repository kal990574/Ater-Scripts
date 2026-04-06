using UnityEngine;

public abstract class SubJumpScareDefinitionSOBase : ScriptableObject
{
    public abstract SubJumpScareCommonData Common { get; }

    public string Id
    {
        get
        {
            if (Common == null)
            {
                return string.Empty;
            }

            return Common.Id;
        }
    }

    public string DisplayName
    {
        get
        {
            if (Common == null)
            {
                return string.Empty;
            }

            return Common.DisplayName;
        }
    }

    public ESubJumpScareType Type
    {
        get
        {
            if (Common == null)
            {
                return ESubJumpScareType.None;
            }

            return Common.Type;
        }
    }

    public ESubJumpScareIntensity Intensity
    {
        get
        {
            if (Common == null)
            {
                return ESubJumpScareIntensity.Weak;
            }

            return Common.Intensity;
        }
    }

    public bool IsValid()
    {
        if (Common == null)
        {
            return false;
        }

        if (string.IsNullOrEmpty(Common.Id) == true)
        {
            return false;
        }

        return true;
    }
    
    protected SubJumpScareCommonData EnsureCommon(SubJumpScareCommonData common)
    {
        
        if (common == null)
        {
            common = new SubJumpScareCommonData();
        }

        return common;
    }
}