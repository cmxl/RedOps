using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedOps.Application;
using RedOps.Infrastructure;
using RedOps.Worker.Services;
using RedOps.Worker.Configuration;
using Serilog;
using Serilog.Events;
using Hangfire;
using Hangfire.SqlServer;

var builder = Host.CreateApplicationBuilder(args);

// Configure Serilog  
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File("logs/redops-worker-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddSerilog();

// Configure options
builder.Services.Configure<OutboxProcessorOptions>(
    builder.Configuration.GetSection(OutboxProcessorOptions.SectionName));
builder.Services.Configure<SyncSchedulerOptions>(
    builder.Configuration.GetSection(SyncSchedulerOptions.SectionName));
builder.Services.Configure<HealthCheckOptions>(
    builder.Configuration.GetSection(HealthCheckOptions.SectionName));

// Add application layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Register background job classes
builder.Services.AddScoped<SyncJob>();

// Add background services
builder.Services.AddHostedService<OutboxProcessorService>();
builder.Services.AddHostedService<SyncSchedulerService>();
builder.Services.AddHostedService<HealthCheckService>();

// Add Hangfire
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = Environment.ProcessorCount;
    options.Queues = new[] { "sync", "outbox", "default" };
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<RedOps.Infrastructure.Data.ApplicationDbContext>();

var host = builder.Build();

Log.Information("RedOps Worker starting up...");

try
{
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "RedOps Worker terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}