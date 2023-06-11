using Amazon.SQS;
using Amazon.SQS.Model;

var cts = new CancellationTokenSource();
AmazonSQSClient sqsClient = new();

GetQueueUrlResponse queueUrlResponse = await sqsClient.GetQueueUrlAsync("customers");

ReceiveMessageRequest receiveMessageRequest = new()
{
    QueueUrl = queueUrlResponse.QueueUrl,
    AttributeNames = new List<string> { "All" },
    MessageAttributeNames = new List<string> { "All" }
};

while (!cts.IsCancellationRequested)
{
    var response = await sqsClient.ReceiveMessageAsync(receiveMessageRequest, cts.Token);
    foreach (var message in response.Messages)
    {
        Console.WriteLine($"Message Id: {message.MessageId}");
        Console.WriteLine($"Message Body: {message.Body}");

        await sqsClient.DeleteMessageAsync(queueUrlResponse.QueueUrl, message.ReceiptHandle);
    }

    await Task.Delay(3000);
}

Console.ReadKey();