using Azure.Messaging.ServiceBus;

namespace FruitsBasket.Infrastructure.MessageBroker;

public class BasketStatsPublisher(ServiceBusClient client, IQueueNameProvider provider) : IPublisher<Guid>
{
    private const string QUEUE_KEY = "BasketStats";
    private readonly ServiceBusSender _sender = client.CreateSender(provider.GetQueueName(QUEUE_KEY));

    public async Task PublishAsync(Guid id)
    {
        await _sender.SendMessageAsync(new ServiceBusMessage(id.ToString()));
    }
}