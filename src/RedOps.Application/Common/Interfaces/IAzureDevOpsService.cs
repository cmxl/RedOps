using RedOps.Application.Common.Models;

namespace RedOps.Application.Common.Interfaces;

public interface IAzureDevOpsService
{
    Task<IEnumerable<AzureDevOpsProject>> GetProjectsAsync(CancellationToken cancellationToken = default);
    Task<AzureDevOpsProject?> GetProjectAsync(string projectName, CancellationToken cancellationToken = default);
    Task<IEnumerable<AzureWorkItem>> GetWorkItemsAsync(string projectName, DateTime? updatedSince = null, CancellationToken cancellationToken = default);
    Task<AzureWorkItem?> GetWorkItemAsync(int workItemId, CancellationToken cancellationToken = default);
    Task<AzureWorkItem> CreateWorkItemAsync(string projectName, AzureWorkItemCreateRequest request, CancellationToken cancellationToken = default);
    Task<AzureWorkItem> UpdateWorkItemAsync(int workItemId, AzureWorkItemUpdateRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<AzureComment>> GetCommentsAsync(int workItemId, CancellationToken cancellationToken = default);
    Task<AzureComment> AddCommentAsync(int workItemId, string content, CancellationToken cancellationToken = default);
    Task<IEnumerable<AzurePullRequest>> GetPullRequestsAsync(string projectName, string repositoryName, CancellationToken cancellationToken = default);
    Task LinkPullRequestToWorkItemAsync(int pullRequestId, int workItemId, CancellationToken cancellationToken = default);
}