using System.Text;
using CommandService.Configurations;
using CommandService.EventProcessing;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService;

public class MessageBusEventHandler : IHostedService, IDisposable
{
    private readonly IOptions<RabbitMqConfiguration> configuration;
    private readonly ILogger<MessageBusEventHandler> logger;
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string queueName;
    private const string EchangeName = "trigger";

    public MessageBusEventHandler(IOptions<RabbitMqConfiguration> configuration, ILogger<MessageBusEventHandler> logger, IServiceScopeFactory scopeFactory)
    {
        this.configuration = configuration;
        this.logger = logger;
        this.scopeFactory = scopeFactory;

        var connectionFactory = new ConnectionFactory
        {
            Uri = new Uri($"amqp://guest:guest@{configuration.Value.Host}:{configuration.Value.Port}")
        };
        connection = connectionFactory.CreateConnection();
        channel = connection.CreateModel();
        channel.ExchangeDeclare(exchange: EchangeName, type: ExchangeType.Fanout);
        queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(queue: queueName,
            exchange: EchangeName,
            routingKey: "");
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        
        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += async (_, basicDeliverEventArgs) =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var body = basicDeliverEventArgs.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

            using var scope = scopeFactory.CreateScope();
            var eventProcessor = scope.ServiceProvider.GetRequiredService<IEventProcessor>();
            
            await eventProcessor.ProcessEvent(notificationMessage);
        };

        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        
        logger.LogInformation("Successfully subscribed for message bus messages");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        connection.Dispose();
        channel.Dispose();
    }
}