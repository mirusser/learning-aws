
using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using SnsPublisher;

CustomerCreated customer = new ()
{
    Id = Guid.NewGuid(),
    Email = "example@example.com",
    FullName = "John Doe",
    DateOfBirth = new DateTime(1990, 1, 1),
    GitHubUsername = "JohnDoe"
};

AmazonSimpleNotificationServiceClient snsClient = new();

var topicArnResponse = await snsClient.FindTopicAsync("customers");

PublishRequest publishRequest = new()
{
    TopicArn = topicArnResponse.TopicArn,
    Message = JsonSerializer.Serialize(customer),
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

var response = await snsClient.PublishAsync(publishRequest);

Console.WriteLine(response);

Console.ReadLine();