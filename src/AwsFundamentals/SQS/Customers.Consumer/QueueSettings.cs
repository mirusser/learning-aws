namespace Customers.Consumer;

public sealed class QueueSettings
{
    public const string Key = "Queue";
    public required string Name { get; init; }
}