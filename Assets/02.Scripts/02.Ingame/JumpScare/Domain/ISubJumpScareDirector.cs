public interface ISubJumpScareDirector
{
    ESubJumpScareType Type { get; }
    bool CanExecute(JumpScareRuntimeContext context, SubJumpScareDefinitionSO definition);
    SubJumpScareHandle Execute(JumpScareRuntimeContext context, SubJumpScareDefinitionSO definition, EJumpScareIntensity intensity);
}