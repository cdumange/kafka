
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Models.Avro;
using Producer.Schemas;


using var registry = new CachedSchemaRegistryClient(new SchemaRegistryConfig
{
    Url = "registry:8081",
});

var schema = await registry.GetRegisteredSchemaAsync("simple", 2);

using var client = new ConsumerBuilder<string, Simple>(new ConsumerConfig
{
    AllowAutoCreateTopics = false,
    BootstrapServers = "kafka-1:9092,kafka-0:9092",
    GroupId = "hey-aaa",
    AutoOffsetReset = AutoOffsetReset.Earliest,
}).SetValueDeserializer(new AvroDeserializer<Simple>(registry).AsSyncOverAsync()).Build();

client.Subscribe("simple");

CancellationTokenSource cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    // Prevent the process from terminating.
    e.Cancel = true;
    cts.Cancel();
};


while (true)
{
    var cr = client.Consume(cts.Token);
    Console.WriteLine($"topic: {cr.Topic}, key: {cr.Message.Key}, value: {cr.Message.Value}");
}

