using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace FruitsBasket.Infrastructure.RabbitMQ;

public class RabbitMqProducer : IMessageProducer, IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly RabbitMqConfiguration _configuration;
    private readonly ILogger<RabbitMqProducer> _logger;

    public RabbitMqProducer(IOptions<RabbitMqConfiguration> configuration, ILogger<RabbitMqProducer> logger)
    {
        _configuration = configuration.Value;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = _configuration.HostName,
            UserName = _configuration.UserName,
            Password = _configuration.Password,
        };

        try
        {
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create RabbitMQ connection");
            throw;
        }
    }

    public async Task PublishAsync<T>(string exchangeName, string routingKey, T message)
    {
        await _channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Topic, true);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);
        var properties = new BasicProperties
        {
            Persistent = true,
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };

        await _channel.BasicPublishAsync(exchangeName, routingKey, false, properties, body);
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.CloseAsync();
        await _connection.CloseAsync();
    }
}