namespace RedOps.Domain.ValueObjects;

public record RedmineId(int Value)
{
    public static implicit operator int(RedmineId redmineId) => redmineId.Value;
    public static implicit operator RedmineId(int value) => new(value);
    
    public override string ToString() => Value.ToString();
}