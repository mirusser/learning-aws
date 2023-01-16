using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HelloLambda;

public class Function
{
    /// <summary>
    /// A simple function to test Lambda
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public CreateProductResponse FunctionHandler(
        CreateProductRequest input,
        ILambdaContext context)
    {
        CreateProductResponse response = new()
        {
            ProductId = Guid.NewGuid().ToString(),
            Name = input.Name,
            Description = input.Description,
        };

        return response;
    }
}

public class CreateProductRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class CreateProductResponse
{
    public string? ProductId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}