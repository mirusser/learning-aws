using System.Reflection;
using Amazon.SQS;
using Customers.Consumer;

var builder = WebApplication.CreateBuilder(args);

{
    builder.Services.Configure<QueueSettings>(builder.Configuration.GetSection(QueueSettings.Key));
    builder.Services.AddSingleton<IAmazonSQS, AmazonSQSClient>();
    builder.Services.AddHostedService<QueueConsumerService>();
    builder.Services.AddMediatR(cfg => 
        cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
}

var app = builder.Build();

{

}

app.Run();