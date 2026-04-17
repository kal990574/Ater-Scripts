using UnityEngine;

public class OnQTEResultInterpreter : SubTensionInterpreterBase
{
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
                            TensionReasons.QteFailed,
                            ETensionChannel.SpikeTension,
                            TensionDeltaConstants.QteFailedSpike));
                    
                    publisher.TryPublish(
                        context => new OnTensionChangedEvent(
                            context,
                            TensionReasons.QteFailed,
                            ETensionChannel.BaseTension,
                            TensionDeltaConstants.QteFailedBase));
                    break;
                }

            case EQuickTimeEventResult.GreatSuccess:
                {
                    publisher.TryPublish(
                        context => new OnTensionChangedEvent(
                            context,
                            TensionReasons.QteGreatSuccess,
                            ETensionChannel.BaseTension,
                            TensionDeltaConstants.QteGreatSuccessBase));
                    break;
                }

            case EQuickTimeEventResult.Success:
                {
                    publisher.TryPublish(
                        context => new OnTensionChangedEvent(
                            context,
                            TensionReasons.QteSuccess,
                            ETensionChannel.BaseTension,
                            TensionDeltaConstants.QteSuccessBase));
                    break;
                }
        }
    }

    
    protected override void Reset()
    {
    }
}
