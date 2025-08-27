namespace RedOps.Domain.Enums;

public enum ConflictType
{
    FieldMismatch,
    DeletedInSource,
    DeletedInTarget,
    ConcurrentModification,
    ValidationError
}