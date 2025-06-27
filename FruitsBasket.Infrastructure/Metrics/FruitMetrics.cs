using Prometheus;

namespace FruitsBasket.Infrastructure.Metrics;

public static class FruitMetrics
{
    public static readonly Gauge ActiveFruitsTotal = Prometheus.Metrics.CreateGauge(
        "fruitsbasket_active_fruits_total",
        "Current number of active fruits in the system",
        new GaugeConfiguration
        {
            LabelNames = ["fruits"]
        }
    );
}