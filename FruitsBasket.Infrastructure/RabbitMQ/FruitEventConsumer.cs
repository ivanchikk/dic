using System.Text;
using System.Text.Json;
using FruitsBasket.Infrastructure.Email;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FruitsBasket.Infrastructure.RabbitMQ;

public class FruitEventConsumer : BackgroundService, IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly RabbitMqConfiguration _configuration;
    private IEmailService _emailService = null!;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<FruitEventConsumer> _logger;

    public FruitEventConsumer(
        IOptions<RabbitMqConfiguration> configuration,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<FruitEventConsumer> logger
    )
    {
        _configuration = configuration.Value;
        _serviceScopeFactory = serviceScopeFactory;
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

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await _channel.ExchangeDeclareAsync(
            exchange: "fruits.events",
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        var queueResult = await _channel.QueueDeclareAsync(
            queue: "",
            durable: false,
            exclusive: true,
            autoDelete: true,
            cancellationToken: cancellationToken);

        await _channel.QueueBindAsync(
            queue: queueResult.QueueName,
            exchange: "fruits.events",
            routingKey: "fruit.*",
            cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = eventArgs.RoutingKey;

                _logger.LogInformation("Received event: {RoutingKey} - {Message}", routingKey, message);

                using var scope = _serviceScopeFactory.CreateScope();
                _emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                await (routingKey switch
                {
                    "fruit.created" => HandleFruitCreatedAsync(message),
                    "fruit.updated" => HandleFruitUpdatedAsync(message),
                    "fruit.deleted" => HandleFruitDeletedAsync(message),
                    _ => HandleUnknownRoutingKeyAsync(routingKey)
                });

                await _channel.BasicAckAsync(eventArgs.DeliveryTag, false, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing message");
                await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, true, cancellationToken);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: queueResult.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);

        var tcs = new TaskCompletionSource();
        await using var registration = cancellationToken.Register(() => tcs.SetResult());
        await tcs.Task;
    }

    private async Task HandleFruitCreatedAsync(string message)
    {
        var fruitEvent = JsonSerializer.Deserialize<FruitEvent>(message)!;

        _logger.LogInformation("Processing fruit created: {FruitEventName}", fruitEvent.fruit.Name);

        await _emailService.SendFruitNotificationAsync(fruitEvent.fruit, "Created");
    }

    private async Task HandleFruitUpdatedAsync(string message)
    {
        var fruitEvent = JsonSerializer.Deserialize<FruitEvent>(message)!;

        _logger.LogInformation("Processing fruit updated: {FruitEventName}", fruitEvent.fruit.Name);

        await _emailService.SendFruitNotificationAsync(fruitEvent.fruit, "Updated");
    }

    private async Task HandleFruitDeletedAsync(string message)
    {
        var fruitEvent = JsonSerializer.Deserialize<FruitEvent>(message)!;

        _logger.LogInformation("Processing fruit deleted: {FruitEventName} ({FruitEventId})", fruitEvent.fruit.Name,
            fruitEvent.fruit.Id);

        await _emailService.SendFruitNotificationAsync(fruitEvent.fruit, "Deleted");
    }

    private async Task HandleUnknownRoutingKeyAsync(string routingKey)
    {
        _logger.LogWarning("Unknown routing key: {RoutingKey}", routingKey);

        await Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.CloseAsync();
        await _connection.CloseAsync();
    }
}