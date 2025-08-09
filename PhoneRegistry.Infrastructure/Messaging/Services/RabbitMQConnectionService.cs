using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace PhoneRegistry.Infrastructure.Messaging.Services;

public class RabbitMQConnectionService : IDisposable
{
    private readonly ILogger<RabbitMQConnectionService> _logger;
    private readonly string _connectionString;
    private IConnection? _connection;
    private IModel? _channel;
    private readonly object _lock = new();

    public RabbitMQConnectionService(IConfiguration configuration, ILogger<RabbitMQConnectionService> logger)
    {
        _logger = logger;
        _connectionString = configuration.GetConnectionString("RabbitMQ") 
            ?? "amqp://admin:admin123@localhost:5672/";
    }

    public IModel GetChannel()
    {
        if (_channel == null || _channel.IsClosed)
        {
            lock (_lock)
            {
                if (_channel == null || _channel.IsClosed)
                {
                    CreateConnection();
                }
            }
        }

        return _channel!;
    }

    private void CreateConnection()
    {
        try
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri(_connectionString);
            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _logger.LogInformation("RabbitMQ connection established successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create RabbitMQ connection");
            throw;
        }
    }

    public void Dispose()
    {
        try
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing RabbitMQ connection");
        }
    }
}
