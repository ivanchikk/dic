using Azure.Messaging.ServiceBus;

namespace FruitsBasket.Infrastructure.MessageBroker;

public class BasketStatsPublisher(ServiceBusClient client, IQueueNameProvider provider) : IPublisher<Guid>
{
    private const string QueueKey = "BasketStats";
    private readonly ServiceBusSender _sender = client.CreateSender(provider.GetQueueName(QueueKey));

    public async Task PublishAsync(Guid id)
    {
        await _sender.SendMessageAsync(new ServiceBusMessage(id.ToString()));
    }
}