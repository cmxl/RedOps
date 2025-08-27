using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using RedOps.Application.Common.Interfaces;
using RedOps.Application.Common.Models;

namespace RedOps.Infrastructure.Services;

public class AzureDevOpsService : IAzureDevOpsService
{
    private readonly VssConnection _connection;
    private readonly WorkItemTrackingHttpClient _workItemClient;
    private readonly ILogger<AzureDevOpsService> _logger;

    public AzureDevOpsService(IConfiguration configuration, ILogger<AzureDevOpsService> logger)
    {
        _logger = logger;
        
        var organizationUrl = configuration["AzureDevOps:OrganizationUrl"] ?? 
            throw new ArgumentException("AzureDevOps:OrganizationUrl not configured");
        var personalAccessToken = configuration["AzureDevOps:PersonalAccessToken"] ?? 
            throw new ArgumentException("AzureDevOps:PersonalAccessToken not configured");

        var credentials = new VssBasicCredential(string.Empty, personalAccessToken);
        _connection = new VssConnection(new Uri(organizationUrl), credentials);
        _workItemClient = _connection.GetClient<WorkItemTrackingHttpClient>();
    }

    public async Task<IEnumerable<AzureDevOpsProject>> GetProjectsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var projectClient = _connection.GetClient<Microsoft.TeamFoundation.Core.WebApi.ProjectHttpClient>();
            var projects = await projectClient.GetProjects();

            return projects.Select(p => new AzureDevOpsProject
            {
                Id = p.Id.ToString(),
                Name = p.Name,
                Description = p.Description,
                State = p.State.ToString(),
                LastUpdateTime = p.LastUpdateTime
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Azure DevOps projects");
            return Enumerable.Empty<AzureDevOpsProject>();
        }
    }

    public async Task<AzureDevOpsProject?> GetProjectAsync(string projectName, CancellationToken cancellationToken = default)
    {
        try
        {
            var projectClient = _connection.GetClient<Microsoft.TeamFoundation.Core.WebApi.ProjectHttpClient>();
            var project = await projectClient.GetProject(projectName);

            return new AzureDevOpsProject
            {
                Id = project.Id.ToString(),
                Name = project.Name,
                Description = project.Description,
                State = project.State.ToString(),
                LastUpdateTime = project.LastUpdateTime
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Azure DevOps project {ProjectName}", projectName);
            return null;
        }
    }

    public async Task<IEnumerable<AzureWorkItem>> GetWorkItemsAsync(string projectName, DateTime? updatedSince = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var wiql = $@"
                SELECT [System.Id], [System.WorkItemType], [System.Title], [System.State], 
                       [System.CreatedDate], [System.ChangedDate], [System.CreatedBy], [System.AssignedTo]
                FROM WorkItems 
                WHERE [System.TeamProject] = '{projectName}'";

            if (updatedSince.HasValue)
            {
                wiql += $" AND [System.ChangedDate] >= '{updatedSince.Value:yyyy-MM-dd}'";
            }

            wiql += " ORDER BY [System.ChangedDate] DESC";

            var query = new Wiql { Query = wiql };
            var queryResult = await _workItemClient.QueryByWiqlAsync(query);

            if (queryResult.WorkItems?.Any() != true)
            {
                return Enumerable.Empty<AzureWorkItem>();
            }

            var ids = queryResult.WorkItems.Select(wi => wi.Id).ToArray();
            var workItems = await _workItemClient.GetWorkItemsAsync(ids, expand: WorkItemExpand.Fields);

            return workItems.Select(wi => new AzureWorkItem
            {
                Id = wi.Id ?? 0,
                WorkItemType = wi.Fields.GetValueOrDefault("System.WorkItemType")?.ToString() ?? "",
                Title = wi.Fields.GetValueOrDefault("System.Title")?.ToString() ?? "",
                Description = wi.Fields.GetValueOrDefault("System.Description")?.ToString(),
                State = wi.Fields.GetValueOrDefault("System.State")?.ToString() ?? "",
                Priority = wi.Fields.GetValueOrDefault("Microsoft.VSTS.Common.Priority")?.ToString(),
                AssignedTo = wi.Fields.GetValueOrDefault("System.AssignedTo")?.ToString(),
                CreatedBy = wi.Fields.GetValueOrDefault("System.CreatedBy")?.ToString() ?? "",
                CreatedDate = DateTime.Parse(wi.Fields.GetValueOrDefault("System.CreatedDate")?.ToString() ?? DateTime.MinValue.ToString()),
                ChangedDate = DateTime.Parse(wi.Fields.GetValueOrDefault("System.ChangedDate")?.ToString() ?? DateTime.MinValue.ToString()),
                Fields = wi.Fields.ToDictionary(f => f.Key, f => f.Value)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Azure DevOps work items for project {ProjectName}", projectName);
            return Enumerable.Empty<AzureWorkItem>();
        }
    }

    public async Task<AzureWorkItem?> GetWorkItemAsync(int workItemId, CancellationToken cancellationToken = default)
    {
        try
        {
            var workItem = await _workItemClient.GetWorkItemAsync(workItemId, expand: WorkItemExpand.Fields);
            
            return new AzureWorkItem
            {
                Id = workItem.Id ?? 0,
                WorkItemType = workItem.Fields.GetValueOrDefault("System.WorkItemType")?.ToString() ?? "",
                Title = workItem.Fields.GetValueOrDefault("System.Title")?.ToString() ?? "",
                Description = workItem.Fields.GetValueOrDefault("System.Description")?.ToString(),
                State = workItem.Fields.GetValueOrDefault("System.State")?.ToString() ?? "",
                Priority = workItem.Fields.GetValueOrDefault("Microsoft.VSTS.Common.Priority")?.ToString(),
                AssignedTo = workItem.Fields.GetValueOrDefault("System.AssignedTo")?.ToString(),
                CreatedBy = workItem.Fields.GetValueOrDefault("System.CreatedBy")?.ToString() ?? "",
                CreatedDate = DateTime.Parse(workItem.Fields.GetValueOrDefault("System.CreatedDate")?.ToString() ?? DateTime.MinValue.ToString()),
                ChangedDate = DateTime.Parse(workItem.Fields.GetValueOrDefault("System.ChangedDate")?.ToString() ?? DateTime.MinValue.ToString()),
                Fields = workItem.Fields.ToDictionary(f => f.Key, f => f.Value)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Azure DevOps work item {WorkItemId}", workItemId);
            return null;
        }
    }

    public Task<AzureWorkItem> CreateWorkItemAsync(string projectName, AzureWorkItemCreateRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Azure DevOps work item creation will be implemented in next phase");
    }

    public Task<AzureWorkItem> UpdateWorkItemAsync(int workItemId, AzureWorkItemUpdateRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Azure DevOps work item updates will be implemented in next phase");
    }

    public Task<IEnumerable<AzureComment>> GetCommentsAsync(int workItemId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Azure DevOps comments will be implemented in next phase");
    }

    public Task<AzureComment> AddCommentAsync(int workItemId, string content, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Azure DevOps comment creation will be implemented in next phase");
    }

    public Task<IEnumerable<AzurePullRequest>> GetPullRequestsAsync(string projectName, string repositoryName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Azure DevOps pull request retrieval will be implemented in next phase");
    }

    public Task LinkPullRequestToWorkItemAsync(int pullRequestId, int workItemId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Pull request linking will be implemented in next phase");
    }
}