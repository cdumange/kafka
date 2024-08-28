
using Confluent.Kafka;


using var client = new ConsumerBuilder<string, string>(new ConsumerConfig
{
    AllowAutoCreateTopics = true,
    BootstrapServers = "kafka-1:9092",
    GroupId = "hey-aaa",
    AutoOffsetReset = AutoOffsetReset.Earliest,

}).Build();

client.Subscribe("test");

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
