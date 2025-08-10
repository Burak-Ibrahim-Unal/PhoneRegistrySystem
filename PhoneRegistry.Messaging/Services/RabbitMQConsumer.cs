using Microsoft.Extensions.Logging;
using PhoneRegistry.Messaging.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace PhoneRegistry.Messaging.Services;

public class RabbitMQConsumer<T> : IDisposable where T : class
{
    private readonly RabbitMQConnectionService _connectionService;
    private readonly ILogger<RabbitMQConsumer<T>> _logger;
    private readonly IMessageConsumer<T> _messageHandler;
    private IModel? _channel;
    private string? _consumerTag;

    public RabbitMQConsumer(
        RabbitMQConnectionService connectionService,
        ILogger<RabbitMQConsumer<T>> logger,
        IMessageConsumer<T> messageHandler)
    {
        _connectionService = connectionService;
        _logger = logger;
        _messageHandler = messageHandler;
    }

    public void StartConsuming(string queueName)
    {
        try
        {
            _channel = _connectionService.GetChannel();
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var jsonMessage = Encoding.UTF8.GetString(body);
                    _logger.LogInformation("Received message from queue {QueueName}: {Message}", queueName, jsonMessage);

                    var message = JsonSerializer.Deserialize<T>(jsonMessage);
                    if (message != null)
                    {
                        await _messageHandler.ConsumeAsync(message);
                        _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        _logger.LogInformation("Message processed successfully from queue {QueueName}", queueName);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to deserialize message from queue {QueueName}", queueName);
                        _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message from queue {QueueName}", queueName);
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _consumerTag = _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            _logger.LogInformation("Started consuming messages from queue {QueueName}", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start consuming from queue {QueueName}", queueName);
            throw;
        }
    }

    public void StopConsuming()
    {
        if (_channel != null && _consumerTag != null)
        {
            _channel.BasicCancel(_consumerTag);
            _logger.LogInformation("Stopped consuming messages");
        }
    }

    public void Dispose()
    {
        StopConsuming();
        _channel?.Dispose();
    }
}


