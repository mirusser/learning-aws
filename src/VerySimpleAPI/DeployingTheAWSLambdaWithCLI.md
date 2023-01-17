# Deploying the AWS Lambda with CLI

resource: https://codewithmukesh.com/blog/hosting-aspnet-core-web-api-with-aws-lambda/

1. Add package: 
```
Amazon.Lambda.AspNetCoreServer.Hosting
```

2. Register it: 
```
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
```

3. Add file: `aws-lambda-tools-defaults.json`

4. Use command (in root directory with `.csproj`):
```powershell
dotnet lambda deploy-function
```

5. You may select an existing IAM Role or create a new one