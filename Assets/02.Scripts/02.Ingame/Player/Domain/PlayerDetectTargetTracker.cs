using UnityEngine;

namespace _02.Scripts.Player
{
    public class PlayerDetectTargetTracker
    {
        private readonly PlayerTargetDetector _detector;

        public PlayerDetectTargetTracker(PlayerTargetDetector detector)
        {
            _detector = detector;
        }

        public IDetectable CurrentTarget { get; private set; }

        public void UpdateTarget(Vector3 origin, Vector3 forward)
        {
            IDetectable nextTarget = _detector.Detect(origin, forward);
            if (ReferenceEquals(CurrentTarget, nextTarget))
            {
                return;
            }

            CurrentTarget?.OnDetectExit();
            CurrentTarget = nextTarget;
            CurrentTarget?.OnDetectEnter();
        }

        public void Clear()
        {
            if (CurrentTarget == null)
            {
                return;
            }

            CurrentTarget.OnDetectExit();
            CurrentTarget = null;
        }
    }
}
