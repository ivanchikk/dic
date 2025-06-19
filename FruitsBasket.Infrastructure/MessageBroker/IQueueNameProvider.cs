namespace FruitsBasket.Infrastructure.MessageBroker;

public interface IQueueNameProvider
{
    string GetQueueName(string key);
}