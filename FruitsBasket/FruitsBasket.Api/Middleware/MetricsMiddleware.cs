using System.Diagnostics;
using FruitsBasket.Api.Fruit.Metrics;

namespace FruitsBasket.Api.Middleware;

public class MetricsMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        var path = context.GetEndpoint()?.DisplayName ?? "unknown";
        var method = context.Request.Method;

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode.ToString();

            FruitMetrics.ApiRequestsTotal
                .WithLabels(method, path, statusCode)
                .Inc();

            FruitMetrics.RequestDuration
                .WithLabels(method, path)
                .Observe(stopwatch.Elapsed.TotalSeconds);
        }
    }
}