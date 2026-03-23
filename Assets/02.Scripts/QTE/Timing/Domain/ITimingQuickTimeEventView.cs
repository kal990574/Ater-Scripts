public interface ITimingQuickTimeEventView : IQuickTimeEventView
{
    void UpdateView(float successZoneSizeProgress, float greatZonePercent, float judgeZoneStartProgress, float needleProgress);
}