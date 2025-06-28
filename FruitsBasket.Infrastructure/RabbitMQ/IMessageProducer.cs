namespace FruitsBasket.Infrastructure.RabbitMQ;

public interface IMessageProducer
{
    Task PublishAsync<T>(string exchangeName, string routingKey, T message);
}