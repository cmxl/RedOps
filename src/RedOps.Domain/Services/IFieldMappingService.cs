using RedOps.Domain.Entities;

namespace RedOps.Domain.Services;

public interface IFieldMappingService
{
    Task<object> MapRedmineToAzureDevOpsAsync(Guid projectId, object redmineData);
    Task<object> MapAzureDevOpsToRedmineAsync(Guid projectId, object azureDevOpsData);
    Task<bool> ValidateMappingAsync(FieldMapping fieldMapping);
    Task<IEnumerable<FieldMapping>> GetDefaultMappingsAsync();
    Task<string> TransformValueAsync(string sourceValue, string? mappingRule);
}