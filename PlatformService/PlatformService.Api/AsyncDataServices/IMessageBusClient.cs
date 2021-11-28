using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PlatformService.Api.PublicModels;
using RabbitMQ.Client;

namespace PlatformService.Api.AsyncDataServices;

public interface IMessageBusClient
{
    void PublishNewPlatform(PlatformPublished platform);
}

public class MessageBusClient : IMessageBusClient, IDisposable
{
    private readonly ConnectionFactory connectionFactory;
    private readonly IConnection connection;
    private readonly IModel channel;
    private const string QueueName = "trigger";

    public MessageBusClient(IOptions<RabbitMqConfiguration> configuration)
    {
        connectionFactory = new ConnectionFactory
        {
            Uri = new Uri($"amqp://guest:guest@{configuration.Value.Host}:{configuration.Value.Port}")
        };
        
        connection = connectionFactory.CreateConnection();
        channel = connection.CreateModel();
        channel.ExchangeDeclare(exchange: QueueName, type: ExchangeType.Fanout);
    }

    public void PublishNewPlatform(PlatformPublished platform)
    {
        var message = JsonSerializer.Serialize(platform);
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: QueueName, routingKey: "", body: body);
    }

    public void Dispose()
    {
        connection.Dispose();
        channel.Dispose();
    }
}

public class RabbitMqConfiguration
{
    public string Host { get; set; }
    public int Port { get; set; }
}