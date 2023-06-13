using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Movies.Api;

//await new DataSeeder().ImportDataAsync();

Movie1 newMovie1 = new() 
{
    Id = Guid.NewGuid(),
    Title = "21 Jump Street",
    AgeRestriction = 18,
    ReleaseYear = 2012,
    RottenTomatoesPercentage = 85
};

Movie2 newMovie2 = new()
{
    Id = Guid.NewGuid(),
    Title = "21 Jump Street",
    AgeRestriction = 18,
    ReleaseYear = 2012,
    RottenTomatoesPercentage = 85
};

var asJson1 = JsonSerializer.Serialize(newMovie1);
var attributeMap1 = Document.FromJson(asJson1).ToAttributeMap();

var asJson2 = JsonSerializer.Serialize(newMovie2);
var attributeMap2 = Document.FromJson(asJson2).ToAttributeMap();

var transactionRequest = new TransactWriteItemsRequest
{
    TransactItems = new List<TransactWriteItem>
    {
        new()
        {
            Put = new Put
            {
                TableName = "movies-year-title",
                Item = attributeMap1
            },
        },
                new()
        {
            Put = new Put
            {
                TableName = "movies-title-rotten",
                Item = attributeMap2
            },
        }
    }
};

var dynamoDbClient = new AmazonDynamoDBClient();
var response = await dynamoDbClient.TransactWriteItemsAsync(transactionRequest);

Console.WriteLine(response);
Console.ReadLine();