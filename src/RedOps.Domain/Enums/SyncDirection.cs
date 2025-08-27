namespace RedOps.Domain.Enums;

[Flags]
public enum SyncDirection
{
    None = 0,
    FromRedmine = 1,
    ToRedmine = 2,
    Bidirectional = FromRedmine | ToRedmine
}