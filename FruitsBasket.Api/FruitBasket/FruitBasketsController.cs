using Asp.Versioning;
using FruitsBasket.Model.FruitBasket;
using Microsoft.AspNetCore.Mvc;

namespace FruitsBasket.Api.FruitBasket;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}")]
public class FruitBasketsController(IFruitBasketOrchestrator orchestrator) : ControllerBase
{
    [HttpGet("basketIds")]
    public async Task<IActionResult> GetAllBasketsAsync()
    {
        var result = await orchestrator.GetAllBasketsAsync();

        return Ok(result);
    }

    [HttpGet("fruitIds")]
    public async Task<IActionResult> GetAllFruitsAsync()
    {
        var result = await orchestrator.GetAllFruitsAsync();

        return Ok(result);
    }

    [HttpGet("baskets/{basketId:guid}/fruits")]
    public async Task<IActionResult> GetAllFruitsByBasketIdAsync(Guid basketId)
    {
        var result = await orchestrator.GetAllFruitsByBasketIdAsync(basketId);

        return Ok(result);
    }

    [HttpPost("baskets/{basketId:guid}/fruits/{fruitId:int}")]
    public async Task<IActionResult> PostAsync(Guid basketId, int fruitId)
    {
        var result = await orchestrator.CreateAsync(basketId, fruitId);

        return Ok(result);
    }

    [HttpDelete("baskets/{basketId:guid}/fruits/{fruitId:int}")]
    public async Task<IActionResult> DeleteAsync(Guid basketId, int fruitId)
    {
        var result = await orchestrator.DeleteAsync(basketId, fruitId);

        return Ok(result);
    }
}