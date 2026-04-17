using UnityEngine;

public class OnPuzzleResultInterpreter : SubTensionInterpreterBase
{
    public OnPuzzleResultInterpreter(Object source) : base(source)
    {
    }

    protected override void Subscribe(GameEventHub hub)
    {
        subscriptions.Add(hub.Subscribe<PuzzleResultRawEvent>(OnPuzzleResult));
    }

    private void OnPuzzleResult(PuzzleResultRawEvent data)
    {
        if (data.Result == EPuzzleResult.Fail)
        {
            publisher.TryPublish(
                context => new OnTensionChangedEvent(
                    context,
                    TensionReasons.PuzzleFailed,
                    ETensionChannel.SpikeTension,
                    TensionDeltaConstants.PuzzleFailedSpike));
            publisher.TryPublish(
                context => new OnTensionChangedEvent(
                    context,
                    TensionReasons.PuzzleFailed,
                    ETensionChannel.BaseTension,
                    TensionDeltaConstants.PuzzleFailedBase));
        }

        if (data.Result == EPuzzleResult.Cancel)
        {
            publisher.TryPublish(
                context => new OnTensionChangedEvent(
                context,
                TensionReasons.PuzzleFailed,
                ETensionChannel.BaseTension,
                TensionDeltaConstants.PuzzleCancelledBase));
        }
        
    }
    
    protected override void Reset()
    {
    }
}
