namespace _02.Scripts.Core.Timer.Domain
{
    public enum TimerState
    {
        Ready,
        Running,
        Warning,
        Critical,
        Expired
    }

    public class ChapterTimerData
    {
        public float RemainingTime { get; set; }
        public float TotalTime { get; private set; }
        public TimerState State { get; set; }

        public int Minutes => (int)(RemainingTime / 60f);
        public int Seconds => (int)(RemainingTime % 60f);

        public ChapterTimerData(float totalSeconds)
        {
            TotalTime = totalSeconds;
            RemainingTime = totalSeconds;
            State = TimerState.Ready;
        }
    }
}