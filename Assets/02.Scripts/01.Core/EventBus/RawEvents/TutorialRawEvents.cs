using _02.Scripts._02.Ingame.Tutorial.Domain;

// TutorialTrigger에서 발행
public readonly struct TutorialStepCompletedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public TutorialStepId StepId { get; }

    public TutorialStepCompletedRawEvent(GameEventContext context, TutorialStepId stepId)
    {
        Context = context;
        StepId = stepId;
    }
}