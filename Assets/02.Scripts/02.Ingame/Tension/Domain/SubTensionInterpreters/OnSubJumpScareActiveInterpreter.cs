using UnityEngine;

public class OnSubJumpScareActiveInterpreter : SubTensionInterpreterBase
{
    public OnSubJumpScareActiveInterpreter(Object source) : base(source)
    {
    }

    protected override void Subscribe(GameEventHub hub)
    {
        subscriptions.Add(hub.Subscribe<SubJumpScareTriggeredRawEvent>(PublishTensionEvent));
    }

    private void PublishTensionEvent(SubJumpScareTriggeredRawEvent data)
    {
        if (data.Result.IsSuccess == false)
        {
            return;
        }

        float tensionDecreaseAmount = GetTensionDecreaseAmount(data.Result);

        if (tensionDecreaseAmount <= 0f)
        {
            return;
        }

        publisher.TryPublish(
            context => new OnTensionChangedEvent(
                context,
                TensionReasons.SubJumpScareTriggered,
                ETensionChannel.BaseTension,
                -tensionDecreaseAmount));
    }

    private float GetTensionDecreaseAmount(SubJumpScareSelectionResult result)
    {
        if (result.Data == null)
        {
            return 0f;
        }

        return result.Data.TensionDecreaseOnTriggered;
    }

    protected override void Reset()
    {
    }
}