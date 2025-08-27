using RedOps.Application.Common.Models;

namespace RedOps.Application.Common.Interfaces;

public interface IRedmineService
{
    Task<IEnumerable<RedmineProject>> GetProjectsAsync(CancellationToken cancellationToken = default);
    Task<RedmineProject?> GetProjectAsync(int projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RedmineWorkItem>> GetWorkItemsAsync(int projectId, DateTime? updatedSince = null, CancellationToken cancellationToken = default);
    Task<RedmineWorkItem?> GetWorkItemAsync(int workItemId, CancellationToken cancellationToken = default);
    Task<RedmineWorkItem> CreateWorkItemAsync(int projectId, RedmineWorkItemCreateRequest request, CancellationToken cancellationToken = default);
    Task<RedmineWorkItem> UpdateWorkItemAsync(int workItemId, RedmineWorkItemUpdateRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<RedmineComment>> GetCommentsAsync(int workItemId, CancellationToken cancellationToken = default);
    Task<RedmineComment> AddCommentAsync(int workItemId, string content, CancellationToken cancellationToken = default);
}