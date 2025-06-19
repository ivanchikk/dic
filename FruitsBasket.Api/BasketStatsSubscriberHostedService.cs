using FruitsBasket.Infrastructure.MessageBroker;

namespace FruitsBasket.Api;

public class BasketStatsSubscriberHostedService(ISubscriber subscriber) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await subscriber.SubscribeAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}