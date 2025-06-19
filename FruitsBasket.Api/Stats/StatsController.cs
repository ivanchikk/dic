using Asp.Versioning;
using FruitsBasket.Infrastructure.MessageBroker;
using Microsoft.AspNetCore.Mvc;

namespace FruitsBasket.Api.Stats;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class StatsController(IStatsStore<Guid> statsStore) : ControllerBase
{
    [HttpGet("baskets")]
    public IActionResult GetAllBasketStats()
    {
        var result = statsStore.GetAll();

        return Ok(result);
    }
}