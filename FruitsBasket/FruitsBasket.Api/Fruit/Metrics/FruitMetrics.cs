using Prometheus;

namespace FruitsBasket.Api.Fruit.Metrics;

public static class FruitMetrics
{
    public static readonly Counter ApiRequestsTotal = Prometheus.Metrics.CreateCounter(
        "fruitsbasket_api_requests_total",
        "Total number of HTTP requests",
        new CounterConfiguration
        {
            LabelNames = ["method", "endpoint", "status_code"]
        }
    );

    public static readonly Histogram RequestDuration = Prometheus.Metrics.CreateHistogram(
        "fruitsbasket_request_duration_seconds",
        "HTTP request execution time in seconds",
        new HistogramConfiguration
        {
            LabelNames = ["method", "endpoint"],
            Buckets = Histogram.ExponentialBuckets(0.01, 2, 10)
        }
    );

    public static readonly Counter FruitOperationsTotal = Prometheus.Metrics.CreateCounter(
        "fruitsbasket_fruit_operations_total",
        "Total number of fruit-related operations",
        new CounterConfiguration
        {
            LabelNames = ["operation"]
        }
    );

    public static readonly Gauge ActiveFruitsTotal = Prometheus.Metrics.CreateGauge(
        "fruitsbasket_active_fruits_total",
        "Current number of active fruits in the system",
        new GaugeConfiguration
        {
            LabelNames = ["Fruits"]
        }
    );
}