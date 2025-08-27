namespace RedOps.Worker.Configuration;

public class OutboxProcessorOptions
{
    public const string SectionName = "OutboxProcessor";

    public int ProcessingIntervalSeconds { get; set; } = 30;
    public int BatchSize { get; set; } = 100;
    public int MaxRetryAttempts { get; set; } = 3;
    public int RetentionDays { get; set; } = 7;
}