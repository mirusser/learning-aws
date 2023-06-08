using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Customers.Consumer.Messages;
using MediatR;
using Microsoft.Extensions.Options;

namespace Customers.Consumer;

public sealed class QueueConsumerService : BackgroundService
{
    private readonly IAmazonSQS sqs;
    private readonly QueueSettings queueSettings;
    private readonly IMediator mediator;
    private readonly ILogger<QueueConsumerService> logger;

    public QueueConsumerService(
        IAmazonSQS sqs,
        IOptions<QueueSettings> queueSettings,
        IMediator mediator,
        ILogger<QueueConsumerService> logger)
    {
        this.sqs = sqs;
        this.queueSettings = queueSettings.Value;
        this.mediator = mediator;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        GetQueueUrlResponse queueUrlResponse =
            await sqs.GetQueueUrlAsync(queueSettings.Name, stoppingToken);

        ReceiveMessageRequest receiveMessageRequest = new()
        {
            QueueUrl = queueUrlResponse.QueueUrl,
            AttributeNames = new List<string> { "ALL" },
            MessageAttributeNames = new List<string> { "ALL" },
            MaxNumberOfMessages = 1
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await sqs.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);

            foreach (var message in response.Messages)
            {
                var messageType = message.MessageAttributes["MessageType"].StringValue;

                var type = Type.GetType($"Customers.Consumer.Messages.{messageType}");

                if (type is null)
                {
                    logger.LogWarning("Unknown message type: {MessageType}", messageType);
                    continue;
                }

                var typedMessage = (ISqsMessage?)JsonSerializer.Deserialize(message.Body, type);
                try
                {
                    await mediator.Send(typedMessage, stoppingToken);

                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Message failed during processing");
                    continue;
                }

                await sqs.DeleteMessageAsync(
                    queueUrlResponse.QueueUrl,
                    message.ReceiptHandle,
                    stoppingToken);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}