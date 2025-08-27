namespace RedOps.Worker.Configuration;

public class HealthCheckOptions
{
    public const string SectionName = "HealthCheck";

    public int CheckIntervalMinutes { get; set; } = 5;
    public long MaxMemoryMB { get; set; } = 1024; // 1 GB
    public int DatabaseTimeoutSeconds { get; set; } = 30;
    public int ExternalApiTimeoutSeconds { get; set; } = 10;
}