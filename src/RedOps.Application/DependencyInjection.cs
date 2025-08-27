using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using RedOps.Application.Common.Behaviors;
using RedOps.Application.Projects.Commands.CreateProject;
using RedOps.Domain.Events;
using System.Reflection;

namespace RedOps.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediator(options =>
        {
            // Only scan the Application assembly where handlers are located
            options.Assemblies = [Assembly.GetExecutingAssembly()];
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.PipelineBehaviors = [typeof(CachingBehavior<,>)];
        });

        services.AddMemoryCache();
        
        return services;
    }
}