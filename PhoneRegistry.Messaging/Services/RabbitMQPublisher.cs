using Microsoft.Extensions.Logging;
using PhoneRegistry.Messaging.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PhoneRegistry.Messaging.Services;

public class RabbitMQPublisher : IMessagePublisher
{
    private readonly RabbitMQConnectionService _connectionService;
    private readonly ILogger<RabbitMQPublisher> _logger;

    public RabbitMQPublisher(RabbitMQConnectionService connectionService, ILogger<RabbitMQPublisher> logger)
    {
        _connectionService = connectionService;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T message, string queueName, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var channel = _connectionService.GetChannel();

            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var jsonMessage = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);
            _logger.LogInformation("Message published to queue {QueueName}: {MessageType}", queueName, typeof(T).Name);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message to queue {QueueName}", queueName);
            throw;
        }
    }
}


