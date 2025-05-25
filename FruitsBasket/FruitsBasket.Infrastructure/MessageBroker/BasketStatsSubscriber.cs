using Azure.Messaging.ServiceBus;

namespace FruitsBasket.Infrastructure.MessageBroker;

public class BasketStatsSubscriber(ServiceBusClient client, IStatsStore<Guid> statsStore, IQueueNameProvider provider)
    : ISubscriber
{
    private const string QueueKey = "BasketStats";
    private readonly ServiceBusProcessor _processor = client.CreateProcessor(provider.GetQueueName(QueueKey));

    public async Task SubscribeAsync()
    {
        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;

        await _processor.StartProcessingAsync();
    }

    private async Task ProcessErrorAsync(ProcessErrorEventArgs arg)
    {
        Console.WriteLine(arg.Exception.Message);
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs arg)
    {
        var basketId = Guid.Parse(arg.Message.Body.ToString());
        statsStore.Add(basketId);

        await arg.CompleteMessageAsync(arg.Message);
    }
}