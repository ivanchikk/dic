using System.Net;
using System.Text.Json;

namespace FruitsBasket.Api;

public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Exception message: {ExceptionMessage}", exception.Message);
            await HandleExceptionAsync(context, exception);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode code;
        string result;

        switch (exception)
        {
            default:
                code = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new { error = exception.Message });
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsync(result);
    }
}