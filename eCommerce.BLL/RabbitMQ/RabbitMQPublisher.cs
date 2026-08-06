using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace eCommerce.BLL.RabbitMQ;

public class RabbitMQPublisher : IRabbitMQPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly string _productExchangeName;
    private readonly ILogger<RabbitMQPublisher> _logger;

    public RabbitMQPublisher(
        IOptions<RabbitMQOptions> rabbitMQOptions,
        ILogger<RabbitMQPublisher> logger)
    {
        _logger = logger;
        var options = rabbitMQOptions.Value;

        _productExchangeName = options.RABBITMQ_PRODUCT_EXCHANGE;

        try
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName = options.RABBITMQ_HOST,
                Password = options.RABBITMQ_PASSWORD,
                Port = Convert.ToInt32(options.RABBITMQ_PORT),
                UserName = options.RABBITMQ_USERNAME
            };

            _connection = connectionFactory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

            _logger.LogInformation("RabbitMQPublisher successfully connected to {Host}:{Port}.", options.RABBITMQ_HOST, options.RABBITMQ_PORT);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "RabbitMQPublisher failed to connect to RabbitMQ at {Host}:{Port}.", options.RABBITMQ_HOST, options.RABBITMQ_PORT);
            throw; // Re-throw to prevent the application from starting in an invalid state
        }
    }

    public async Task PublishAsync<T>(T message, string routingKey) where T : class
    {
        try
        {
            var messageBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            var exchangeName = _productExchangeName;

            // Ensure the exchange exists before publishing
            await _channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Direct, durable: true);

            // Publish the message
            await _channel.BasicPublishAsync(exchange: exchangeName, routingKey: routingKey, body: messageBytes);

            _logger.LogInformation("Successfully published message of type {MessageType} to exchange '{ExchangeName}' with routing key '{RoutingKey}'.", typeof(T).Name, exchangeName, routingKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message of type {MessageType} to exchange '{ExchangeName}' with routing key '{RoutingKey}'.", typeof(T).Name, _productExchangeName, routingKey);
            throw;
        }
    }

    public void Dispose()
    {
        _logger.LogInformation("Disposing RabbitMQPublisher resources.");
        _channel?.Dispose();
        _connection?.Dispose();
    }
}