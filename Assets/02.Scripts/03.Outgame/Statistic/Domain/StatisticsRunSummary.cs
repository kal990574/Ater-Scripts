public readonly struct StatisticsRunSummary
{
    public int SonarUseCount { get; }
    public int LidarRestoreCount { get; }
    public int AiQuestionCount { get; }
    public float StartTime { get; }
    public float EndTime { get; }
    public float PlayTime { get; }

    public StatisticsRunSummary(CurrentRunStatistics currentRun)
    {
        if (currentRun == null)
        {
            SonarUseCount = 0;
            LidarRestoreCount = 0;
            AiQuestionCount = 0;
            StartTime = 0f;
            EndTime = 0f;
            PlayTime = 0f;
            return;
        }

        SonarUseCount = currentRun.SonarUseCount;
        LidarRestoreCount = currentRun.LidarRestoreCount;
        AiQuestionCount = currentRun.AiQuestionCount;
        StartTime = currentRun.StartTime;
        EndTime = currentRun.EndTime;
        PlayTime = EndTime - StartTime;
    }
}
