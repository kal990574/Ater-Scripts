using UnityEngine;

public class OnPuzzleResultInterpreter : SubTensionInterpreterBase
{
    private const float PUZZLEFAILED_SPIKEDELTA = 30;
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
                    PUZZLEFAILED_SPIKEDELTA));
            publisher.TryPublish(
                context => new OnTensionChangedEvent(
                    context,
                    TensionReasons.PuzzleFailed,
                    ETensionChannel.BaseTension,
                    5f));
        }

        if (data.Result == EPuzzleResult.Cancel)
        {
            publisher.TryPublish(
                context => new OnTensionChangedEvent(
                    context,
                    TensionReasons.PuzzleFailed,
                    ETensionChannel.BaseTension,
                    10f));
        }
        
    }
    
    protected override void Reset()
    {
    }
}