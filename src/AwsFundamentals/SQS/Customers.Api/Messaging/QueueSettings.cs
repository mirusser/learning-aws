namespace Customers.Api.Messaging;

public sealed class QueueSettings
{
    public const string Key = "Queue";
    public required string Name { get; init; }
}