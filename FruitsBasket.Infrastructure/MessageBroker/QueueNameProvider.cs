using Microsoft.Extensions.Configuration;

namespace FruitsBasket.Infrastructure.MessageBroker;

public class QueueNameProvider(IConfiguration configuration) : IQueueNameProvider
{
    public string GetQueueName(string key)
    {
        return configuration[$"ServiceBus:Queues:{key}"]
               ?? throw new InvalidOperationException($"Queue name for key '{key}' not found");
    }
}