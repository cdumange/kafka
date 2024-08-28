using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var b = new ProducerBuilder<string, string>(new ProducerConfig
{
    AllowAutoCreateTopics = true,
    BootstrapServers = "kafka-1:9092",
});

var client = b.Build();


app.MapPost("/post/{topic}", async (ILogger<string> logger, string topic, [FromBody] string value) =>
{
    logger.LogDebug("received {value}", value);
    var res = await client.ProduceAsync(topic, new Message<string, string>
    {
        Key = Guid.NewGuid().ToString(),
        Value = value,
    });

    logger.LogDebug("returned {res}", res);

    return res;
})
.WithName("Send broadcast to kafka")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
