using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AuthLambda;

public class Function
{
    private const string key = "S0M3RAN0MS3CR3T!1!MAG1C!1!";

    public async Task<string> GenerateTokenAsync(
        APIGatewayHttpApiV2ProxyRequest request,
        ILambdaContext context)
    {
        var tokenRequest = JsonConvert.DeserializeObject<User>(request.Body);
        AmazonDynamoDBClient client = new();
        DynamoDBContext dBContext = new(client);

        var user =
            await dBContext.LoadAsync<User>(tokenRequest?.Email)
            ?? throw new Exception("User not found");

        if (user.Password != tokenRequest?.Password) throw new Exception("Invalid Credentials!");

        var token = GenerateJWT(user);

        return token;
    }

    private string GenerateJWT(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.Username)
        };
        byte[] secret = Encoding.UTF8.GetBytes(key);
        SigningCredentials signingCredentials = new(
            new SymmetricSecurityKey(secret),
            SecurityAlgorithms.HmacSha256);
        JwtSecurityToken token = new(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: signingCredentials);
        JwtSecurityTokenHandler tokenHandler = new();

        return tokenHandler.WriteToken(token);
    }

    public APIGatewayCustomAuthorizerResponse ValidateTokenAsync(
        APIGatewayCustomAuthorizerRequest request,
        ILambdaContext context)
    {
        var authToken = request.Headers["authorization"];
        var claimsPrincipal = GetClaimsPrincipal(authToken);
        var effect = claimsPrincipal is null
            ? "Deny"
            : "Allow";
        var principalId = claimsPrincipal is null
            ? "401"
            : claimsPrincipal?.FindFirst(ClaimTypes.Name)?.Value;

        return new()
        {
            PrincipalID = principalId,
            PolicyDocument = new APIGatewayCustomAuthorizerPolicy()
            {
                Statement = new List<APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement>
                {
                    new()
                    {
                        Effect = effect,
                        //TODO: change to your info here:
                        Resource = new HashSet<string> { "arn:aws:execute-api:<aws-region>:<aws-account-id>:<amazon-gateway-id>/*/*" },
                        Action = new HashSet<string> { "execute-api:Invoke" }
                    }
                }
            }
        };
    }

    private ClaimsPrincipal GetClaimsPrincipal(string authToken)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        TokenValidationParameters validationParams = new()
        {
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidateIssuer = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };

        try
        {
            return tokenHandler.ValidateToken(authToken, validationParams, out var _);
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}