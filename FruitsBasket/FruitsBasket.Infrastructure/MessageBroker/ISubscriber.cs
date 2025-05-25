namespace FruitsBasket.Infrastructure.MessageBroker;

public interface ISubscriber
{
    Task SubscribeAsync();
}