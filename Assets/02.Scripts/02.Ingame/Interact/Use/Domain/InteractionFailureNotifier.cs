using UnityEngine;

public static class InteractionFailureNotifier
{
    public static void Notify(InteractionFailSO failMessage, int messageIndex)
    {
        if (failMessage == null)
        {
            return;
        }

        if (!failMessage.TryGetMessage(messageIndex, out string message))
        {
            return;
        }

        GameEventHub hub = GameEventHub.Instance;
        if (hub == null)
        {
            return;
        }

        hub.Publish(new InteractionFailedRawEvent(default, message));
    }
}
