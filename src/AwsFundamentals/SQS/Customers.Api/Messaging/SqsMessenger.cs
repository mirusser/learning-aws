using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;

namespace Customers.Api.Messaging;

public sealed class SqsMessenger : ISqsMessenger
{
    private readonly IAmazonSQS sqs;
    private readonly IOptions<QueueSettings> queueSettings;
    private string? queueUrl;

    public SqsMessenger(
        IAmazonSQS sqs,
        IOptions<QueueSettings> queueSettings)
    {
        this.sqs = sqs;
        this.queueSettings = queueSettings;
    }

    public async Task<SendMessageResponse> SendMessageAsync<T>(T request)
    {
        var queueUrl = await GetQueueUrlAsync<T>();

        var sendMessageRequest = new SendMessageRequest()
        {
            QueueUrl = queueUrl,
            MessageBody = JsonSerializer.Serialize(request),
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

        return await sqs.SendMessageAsync(sendMessageRequest);
    }

    private async Task<string?> GetQueueUrlAsync<T>()
    {
        if (string.IsNullOrEmpty(queueUrl))
        {
            GetQueueUrlResponse queueUrlResponse =
                await sqs.GetQueueUrlAsync(queueSettings.Value.Name);

            if (!string.IsNullOrEmpty(queueUrlResponse.QueueUrl))
            {
                queueUrl = queueUrlResponse.QueueUrl;
            }
        }

        return queueUrl;
    }
}