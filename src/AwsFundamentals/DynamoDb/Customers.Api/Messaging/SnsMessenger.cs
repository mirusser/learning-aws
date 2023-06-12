using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Options;

namespace Customers.Api.Messaging;

public sealed class SnsMessenger : ISnsMessenger
{
    private readonly IAmazonSimpleNotificationService sns;
    private readonly IOptions<TopicSettings> topicSettings;
    private string? topicArn;

    public SnsMessenger(
        IAmazonSimpleNotificationService sns,
        IOptions<TopicSettings> topicSettings)
    {
        this.sns = sns;
        this.topicSettings = topicSettings;
    }

    public async Task<PublishResponse> PublishMessageAsync<T>(T request)
    {
        var topicArn = await GetTopicArn<T>();

        var sendMessageRequest = new PublishRequest()
        {
            TopicArn = topicArn,
            Message = JsonSerializer.Serialize(request),
            MessageAttributes = new()
            {
                {
                    "MessageType",
                    new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = typeof(T).Name
                    }
                }
            }
        };

        return await sns.PublishAsync(sendMessageRequest);
    }

    private async ValueTask<string?> GetTopicArn<T>()
    {
        if (string.IsNullOrEmpty(topicArn))
        {
            var topicArnResponse =
                await sns.FindTopicAsync(topicSettings.Value.Name);

            if (!string.IsNullOrEmpty(topicArnResponse.TopicArn))
            {
                topicArn = topicArnResponse.TopicArn;
            }
        }

        return topicArn;
    }
}