namespace RedOps.Worker.Configuration;

public class SyncSchedulerOptions
{
    public const string SectionName = "SyncScheduler";

    public int ScheduleIntervalMinutes { get; set; } = 15;
    public int MinimumSyncIntervalMinutes { get; set; } = 30;
    public string AutoSyncCronExpression { get; set; } = "0 */30 * * * *"; // Every 30 minutes
    public int OperationRetentionDays { get; set; } = 30;
}