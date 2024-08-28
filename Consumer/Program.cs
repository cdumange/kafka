using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Confluent.Kafka;
using Consumer.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using var client = new ConsumerBuilder<string, string>(new ConsumerConfig
{
    AllowAutoCreateTopics = true,
    BootstrapServers = "127.0.0.1:9092",
    GroupId = "hey-aa",
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
