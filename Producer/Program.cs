using Avro;
using Avro.Generic;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.AspNetCore.Mvc;
using Producer.Schemas;

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

using var registry = new CachedSchemaRegistryClient(new SchemaRegistryConfig
{
    Url = "registry:8081"
});

using var client = new ProducerBuilder<string, GenericRecord>(new ProducerConfig
{
    AllowAutoCreateTopics = true,
    BootstrapServers = "kafka-1:9092,kafka-0:9092",
}).SetValueSerializer(new AvroSerializer<GenericRecord>(registry))
    .Build();


app.MapPost("/post/{topic}/{version}", async (ILogger<string> logger, string topic, int version, [FromBody] Simple value) =>
{
    logger.LogDebug("received {value}", value);
    var schema = await registry.GetRegisteredSchemaAsync(topic, version);
    logger.LogDebug($"topic found for {topic}:{version} = {schema.SchemaString}");

    var s = Avro.Schema.Parse(schema.SchemaString);
    if (s is RecordSchema recorded)
    {
        var record = new GenericRecord(recorded);
        record.Add(0, value.ID);
        record.Add(1, value.Value);

        var res = await client.ProduceAsync(topic, new Message<string, GenericRecord>
        {
            Key = Guid.NewGuid().ToString(),
            Value = record,
        });

        logger.LogDebug("returned {res}", res);
        return res;
    }
    else
    {
        return null;
    }
})
.WithName("Send broadcast to kafka")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
