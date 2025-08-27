namespace RedOps.Domain.ValueObjects;

public record AzureDevOpsId(int Value)
{
    public static implicit operator int(AzureDevOpsId azureId) => azureId.Value;
    public static implicit operator AzureDevOpsId(int value) => new(value);
    
    public override string ToString() => Value.ToString();
}