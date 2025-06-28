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

public class FruitEventConsumer(
    IOptions<RabbitMqConfiguration> configuration,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<FruitEventConsumer> logger)
    : BackgroundService, IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;
    private string? _queueName;
    private readonly RabbitMqConfiguration _configuration = configuration.Value;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await InitializeRabbitMqConnectionAsync();
            await SetupExchangeAndQueueAsync(cancellationToken);
            await StartConsumingAsync(cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to initialize RabbitMQ consumer");
            throw;
        }
    }

    private async Task InitializeRabbitMqConnectionAsync()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration.HostName,
            UserName = _configuration.UserName,
            Password = _configuration.Password,
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
    }

    private async Task SetupExchangeAndQueueAsync(CancellationToken cancellationToken)
    {
        if (_channel == null)
            throw new InvalidOperationException("Channel is not initialized");

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

        _queueName = queueResult.QueueName;

        await _channel.QueueBindAsync(
            queue: queueResult.QueueName,
            exchange: "fruits.events",
            routingKey: "fruit.*",
            cancellationToken: cancellationToken);
    }

    private async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        if (_channel == null)
            throw new InvalidOperationException("Channel is not initialized");
        if (string.IsNullOrEmpty(_queueName))
            throw new InvalidOperationException("Queue name is not initialized");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnMessageReceivedAsync;

        await _channel.BasicConsumeAsync(
            queue: _queueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);

        await WaitForCancellationAsync(cancellationToken);
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        try
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = eventArgs.RoutingKey;

            logger.LogInformation("Received event: {RoutingKey} - {Message}", routingKey, message);

            await ProcessMessageAsync(message, routingKey);
            await _channel!.BasicAckAsync(eventArgs.DeliveryTag, false, CancellationToken.None);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error processing message");
            await _channel!.BasicNackAsync(eventArgs.DeliveryTag, false, true, CancellationToken.None);
        }
    }

    private async Task ProcessMessageAsync(string message, string routingKey)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        await (routingKey switch
        {
            "fruit.created" => HandleFruitCreatedAsync(message, emailService),
            "fruit.updated" => HandleFruitUpdatedAsync(message, emailService),
            "fruit.deleted" => HandleFruitDeletedAsync(message, emailService),
            _ => HandleUnknownRoutingKeyAsync(routingKey)
        });
    }

    private static async Task WaitForCancellationAsync(CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource();
        await using var registration = cancellationToken.Register(() => tcs.SetResult());
        await tcs.Task;
    }

    private async Task HandleFruitCreatedAsync(string message, IEmailService emailService)
    {
        var fruitEvent = JsonSerializer.Deserialize<FruitEvent>(message)!;

        logger.LogInformation("Processing fruit created: {FruitEventName}", fruitEvent.fruit.Name);

        await emailService.SendFruitNotificationAsync(fruitEvent.fruit, "Created");
    }

    private async Task HandleFruitUpdatedAsync(string message, IEmailService emailService)
    {
        var fruitEvent = JsonSerializer.Deserialize<FruitEvent>(message)!;

        logger.LogInformation("Processing fruit updated: {FruitEventName}", fruitEvent.fruit.Name);

        await emailService.SendFruitNotificationAsync(fruitEvent.fruit, "Updated");
    }

    private async Task HandleFruitDeletedAsync(string message, IEmailService emailService)
    {
        var fruitEvent = JsonSerializer.Deserialize<FruitEvent>(message)!;

        logger.LogInformation("Processing fruit deleted: {FruitEventName} ({FruitEventId})", fruitEvent.fruit.Name,
            fruitEvent.fruit.Id);

        await emailService.SendFruitNotificationAsync(fruitEvent.fruit, "Deleted");
    }

    private async Task HandleUnknownRoutingKeyAsync(string routingKey)
    {
        logger.LogWarning("Unknown routing key: {RoutingKey}", routingKey);

        await Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
        {
            await _channel.CloseAsync();
        }

        if (_connection != null)
        {
            await _connection.CloseAsync();
        }
    }
}