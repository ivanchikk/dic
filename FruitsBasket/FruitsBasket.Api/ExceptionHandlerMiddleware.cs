using System.Net;
using System.Text.Json;
using FruitsBasket.Orchestrator.Exceptions;

namespace FruitsBasket.Api;

public class ExceptionHandlerMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode code;
        string result;

        switch (exception)
        {
            case NotFoundException notFoundException:
                code = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new { error = notFoundException.Message });
                break;
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