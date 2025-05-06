using System.Diagnostics;
using System.Text.RegularExpressions;
using FruitsBasket.Api.Fruit.Metrics;

namespace FruitsBasket.Api.Middleware;

public class MetricsMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "unknown";
        path = Regex.Replace(path, @"/Fruits/\d+$", "/Fruits/{id}");

        if (!path.Contains("/Fruits"))
        {
            await next(context);
            return;
        }

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
                .WithLabels(method, path, statusCode)
                .Observe(stopwatch.Elapsed.TotalSeconds);
        }
    }
}