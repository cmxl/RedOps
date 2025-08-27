using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedOps.Application.Common.Interfaces;
using RedOps.Domain.Repositories;
using RedOps.Infrastructure.Data;
using RedOps.Infrastructure.Repositories;
using RedOps.Infrastructure.Services;

namespace RedOps.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        // Repositories
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IWorkItemRepository, WorkItemRepository>();
        services.AddScoped<ISyncOperationRepository, SyncOperationRepository>();
        services.AddScoped<ISyncConflictRepository, SyncConflictRepository>();
        services.AddScoped<IOutboxEventRepository, OutboxEventRepository>();

        // External Services
        services.AddScoped<IRedmineService, RedmineService>();
        services.AddScoped<IAzureDevOpsService, AzureDevOpsService>();
        
        // Core Services
        services.AddScoped<ISyncOrchestrator, SyncOrchestrator>();

        // HTTP Clients with Polly
        services.AddHttpClient<RedmineService>(client =>
        {
            var baseUrl = configuration["Redmine:BaseUrl"];
            if (!string.IsNullOrEmpty(baseUrl))
            {
                client.BaseAddress = new Uri(baseUrl);
            }
        }).AddPolicyHandler(RedOpsHttpPolicyExtensions.GetRetryPolicy())
          .AddPolicyHandler(RedOpsHttpPolicyExtensions.GetCircuitBreakerPolicy());

        return services;
    }
}