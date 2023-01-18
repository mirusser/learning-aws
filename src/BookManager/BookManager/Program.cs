using BookManager;

var builder = WebApplication.CreateBuilder(args);

{
    builder.Services.Configure<MongoDBConfig>(builder.Configuration.GetSection("MongoDBConfig"));
    builder.Services.AddScoped<BookService>();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
}

app.Run();