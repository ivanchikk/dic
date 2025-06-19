using Asp.Versioning;
using FruitsBasket.Infrastructure.MessageBroker;
using Microsoft.AspNetCore.Mvc;

namespace FruitsBasket.Api.Stats;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/basket-stats")]
public class BasketStatsController(IStatsStore<Guid> statsStore) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllAsync()
    {
        var result = statsStore.GetAll();

        return Ok(result);
    }
}