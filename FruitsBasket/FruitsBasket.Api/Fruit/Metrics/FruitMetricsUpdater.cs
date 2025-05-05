using FruitsBasket.Model.Fruit;

namespace FruitsBasket.Api.Fruit.Metrics;

public class FruitMetricsUpdater(IServiceProvider serviceProvider) : BackgroundService
{
    private readonly TimeSpan _updateInterval = TimeSpan.FromMinutes(5);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var fruitRepository = scope.ServiceProvider.GetRequiredService<IFruitRepository>();
                var totalFruits = await fruitRepository.CountAsync();

                FruitMetrics.ActiveFruitsTotal.Set(totalFruits);
            }

            await Task.Delay(_updateInterval, cancellationToken);
        }
    }
}