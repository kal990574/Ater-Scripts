using UnityEngine;

public abstract class BaseSubJumpScareDirector : MonoBehaviour, ISubJumpScareDirector
{
    public abstract ESubJumpScareType Type { get; }

    public abstract bool CanExecute(JumpScareRuntimeContext context, SubJumpScareDefinitionSO definition);

    public abstract SubJumpScareHandle Execute(
        JumpScareRuntimeContext context,
        SubJumpScareDefinitionSO definition,
        EJumpScareIntensity intensity);

    protected TDefinition GetDefinition<TDefinition>(SubJumpScareDefinitionSO definition)
        where TDefinition : SubJumpScareDefinitionSO
    {
        return definition as TDefinition;
    }
}