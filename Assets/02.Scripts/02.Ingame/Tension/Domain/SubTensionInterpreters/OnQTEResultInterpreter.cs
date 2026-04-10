using UnityEngine;

public class OnQTEResultInterpreter : SubTensionInterpreterBase
{
    public const float FAILED_SPIKEDELTA = 20;
    public const float GREAT_BASEDELTA = -10;
    public OnQTEResultInterpreter(Object source) : base(source)
    {
    }

    protected override void Subscribe(GameEventHub hub)
    {
        subscriptions.Add(hub.Subscribe<QteRawEvent>(OnQTEResult));
    }

    private void OnQTEResult(QteRawEvent data)
    {
        switch (data.Result)
        {
            case EQuickTimeEventResult.Fail:
                {
                    publisher.TryPublish(
                        context => new OnTensionChangedEvent(
                            context,
                            TensionReasons.QTEFailed,
                            ETensionChannel.SpikeTension,
                            FAILED_SPIKEDELTA));
                    break;
                }

            case EQuickTimeEventResult.GreatSuccess:
                {
                    publisher.TryPublish(
                        context => new OnTensionChangedEvent(
                            context,
                            TensionReasons.QTEGreatSuccess,
                            ETensionChannel.BaseTension,
                            GREAT_BASEDELTA));
                    break;
                }

            case EQuickTimeEventResult.Success:
                {
                    publisher.TryPublish(
                        context => new OnTensionChangedEvent(
                            context,
                            TensionReasons.QTESuccess,
                            ETensionChannel.BaseTension,
                            0));
                    break;
                }
        }
    }

    
    protected override void Reset()
    {
    }
}