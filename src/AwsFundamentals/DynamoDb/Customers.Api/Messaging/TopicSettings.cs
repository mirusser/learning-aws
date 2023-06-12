namespace Customers.Api.Messaging;

public sealed class TopicSettings
{
    public const string Key = "Topic";
    public required string Name { get; init; }
}