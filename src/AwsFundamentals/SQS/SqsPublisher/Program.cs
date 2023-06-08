using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using SqsPublisher;

AmazonSQSClient sqsClient = new();

CustomerCreated customer = new()
{
    Id = Guid.NewGuid(),
    Email = "example@example.com",
    FullName = "John Doe",
    DateOfBirth = new DateTime(1990, 1, 1),
    GitHubUserName = "JohnDoe"
};

GetQueueUrlResponse queueUrlResponse = await sqsClient.GetQueueUrlAsync("customers");

SendMessageRequest sendMessageRequest = new()
{
    QueueUrl = queueUrlResponse.QueueUrl,
    MessageBody = JsonSerializer.Serialize(customer),
    MessageAttributes = new()
    {
        {
            "MessageType", new MessageAttributeValue
            {
                DataType = "String",
                StringValue = nameof(CustomerCreated)
            }
        }
    }
};

var response = await sqsClient.SendMessageAsync(sendMessageRequest);

Console.WriteLine(response);

Console.ReadKey();