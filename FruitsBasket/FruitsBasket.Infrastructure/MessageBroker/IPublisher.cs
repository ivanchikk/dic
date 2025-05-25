namespace FruitsBasket.Infrastructure.MessageBroker;

public interface IPublisher<in T>
{
    Task PublishAsync(T id);
}