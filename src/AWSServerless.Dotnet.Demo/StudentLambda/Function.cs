using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace StudentLambda;

public class Function
{
    public async Task<List<Student>> GetAllStudentsAsync(
        APIGatewayHttpApiV2ProxyRequest request,
        ILambdaContext context)
    {
        AmazonDynamoDBClient client = new();
        DynamoDBContext dBContext = new(client);

        var data = await dBContext
            .ScanAsync<Student>(default)
            .GetRemainingAsync();

        return data;
    }

    public async Task<APIGatewayHttpApiV2ProxyResponse> CreateStudentAsync(
        APIGatewayHttpApiV2ProxyRequest request,
        ILambdaContext context)
    {
        var studentRequest = JsonConvert.DeserializeObject<Student>(request.Body);
        AmazonDynamoDBClient client = new();
        DynamoDBContext dbContext = new(client);

        await dbContext.SaveAsync(studentRequest);

        var message = $"Student with Id {studentRequest?.Id} created";
        LambdaLogger.Log(message);

        return new()
        {
            Body = message,
            StatusCode = 200
        };
    }

    public async Task<Student> GetStudentByIdAsync(
        APIGatewayHttpApiV2ProxyRequest request,
        ILambdaContext context)
    {
        AmazonDynamoDBClient client = new();
        DynamoDBContext dbContext = new(client);
        string idFromPath = request.PathParameters["id"];
        if (int.TryParse(idFromPath, out int id))
        {
            var student =
                await dbContext.LoadAsync<Student>(id)
                ?? throw new Exception("Not Found");

            return student;
        }

        throw new Exception("Proper Id not found in path parameters");
    }
}
