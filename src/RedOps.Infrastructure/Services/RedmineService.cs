using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RedOps.Application.Common.Interfaces;
using RedOps.Application.Common.Models;
using RestSharp;
using System.Text.Json;

namespace RedOps.Infrastructure.Services;

public class RedmineService : IRedmineService
{
    private readonly RestClient _client;
    private readonly ILogger<RedmineService> _logger;
    private readonly string _apiKey;

    public RedmineService(HttpClient httpClient, IConfiguration configuration, ILogger<RedmineService> logger)
    {
        var baseUrl = configuration["Redmine:BaseUrl"] ?? throw new ArgumentException("Redmine:BaseUrl not configured");
        _apiKey = configuration["Redmine:ApiKey"] ?? throw new ArgumentException("Redmine:ApiKey not configured");
        
        var options = new RestClientOptions(baseUrl)
        {
            ThrowOnAnyError = false,
            Timeout = TimeSpan.FromSeconds(30)
        };
        
        _client = new RestClient(httpClient, options);
        _logger = logger;
    }

    public async Task<IEnumerable<RedmineProject>> GetProjectsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new RestRequest("/projects.json")
                .AddParameter("key", _apiKey)
                .AddParameter("limit", 100);

            var response = await _client.ExecuteAsync(request, cancellationToken);
            
            if (!response.IsSuccessful)
            {
                _logger.LogError("Failed to get Redmine projects: {StatusCode} - {Content}", 
                    response.StatusCode, response.Content);
                return Enumerable.Empty<RedmineProject>();
            }

            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(response.Content ?? "{}");
            if (jsonResponse.TryGetProperty("projects", out var projectsElement))
            {
                return JsonSerializer.Deserialize<RedmineProject[]>(projectsElement.GetRawText()) ?? [];
            }

            return Enumerable.Empty<RedmineProject>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Redmine projects");
            return Enumerable.Empty<RedmineProject>();
        }
    }

    public async Task<RedmineProject?> GetProjectAsync(int projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new RestRequest($"/projects/{projectId}.json")
                .AddParameter("key", _apiKey);

            var response = await _client.ExecuteAsync(request, cancellationToken);
            
            if (!response.IsSuccessful)
            {
                _logger.LogError("Failed to get Redmine project {ProjectId}: {StatusCode} - {Content}", 
                    projectId, response.StatusCode, response.Content);
                return null;
            }

            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(response.Content ?? "{}");
            if (jsonResponse.TryGetProperty("project", out var projectElement))
            {
                return JsonSerializer.Deserialize<RedmineProject>(projectElement.GetRawText());
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Redmine project {ProjectId}", projectId);
            return null;
        }
    }

    public async Task<IEnumerable<RedmineWorkItem>> GetWorkItemsAsync(int projectId, DateTime? updatedSince = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new RestRequest("/issues.json")
                .AddParameter("key", _apiKey)
                .AddParameter("project_id", projectId)
                .AddParameter("limit", 100)
                .AddParameter("include", "journals,attachments");

            if (updatedSince.HasValue)
            {
                request.AddParameter("updated_on", $">={updatedSince.Value:yyyy-MM-dd}");
            }

            var response = await _client.ExecuteAsync(request, cancellationToken);
            
            if (!response.IsSuccessful)
            {
                _logger.LogError("Failed to get Redmine work items for project {ProjectId}: {StatusCode} - {Content}", 
                    projectId, response.StatusCode, response.Content);
                return Enumerable.Empty<RedmineWorkItem>();
            }

            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(response.Content ?? "{}");
            if (jsonResponse.TryGetProperty("issues", out var issuesElement))
            {
                return JsonSerializer.Deserialize<RedmineWorkItem[]>(issuesElement.GetRawText()) ?? [];
            }

            return Enumerable.Empty<RedmineWorkItem>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Redmine work items for project {ProjectId}", projectId);
            return Enumerable.Empty<RedmineWorkItem>();
        }
    }

    public async Task<RedmineWorkItem?> GetWorkItemAsync(int workItemId, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new RestRequest($"/issues/{workItemId}.json")
                .AddParameter("key", _apiKey)
                .AddParameter("include", "journals,attachments");

            var response = await _client.ExecuteAsync(request, cancellationToken);
            
            if (!response.IsSuccessful)
            {
                return null;
            }

            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(response.Content ?? "{}");
            if (jsonResponse.TryGetProperty("issue", out var issueElement))
            {
                return JsonSerializer.Deserialize<RedmineWorkItem>(issueElement.GetRawText());
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Redmine work item {WorkItemId}", workItemId);
            return null;
        }
    }

    public async Task<RedmineWorkItem> CreateWorkItemAsync(int projectId, RedmineWorkItemCreateRequest request, CancellationToken cancellationToken = default)
    {
        // Implementation for creating work items
        throw new NotImplementedException("Redmine work item creation will be implemented in next phase");
    }

    public async Task<RedmineWorkItem> UpdateWorkItemAsync(int workItemId, RedmineWorkItemUpdateRequest request, CancellationToken cancellationToken = default)
    {
        // Implementation for updating work items
        throw new NotImplementedException("Redmine work item updates will be implemented in next phase");
    }

    public async Task<IEnumerable<RedmineComment>> GetCommentsAsync(int workItemId, CancellationToken cancellationToken = default)
    {
        // Implementation for getting comments
        throw new NotImplementedException("Redmine comments will be implemented in next phase");
    }

    public async Task<RedmineComment> AddCommentAsync(int workItemId, string content, CancellationToken cancellationToken = default)
    {
        // Implementation for adding comments
        throw new NotImplementedException("Redmine comment creation will be implemented in next phase");
    }
}