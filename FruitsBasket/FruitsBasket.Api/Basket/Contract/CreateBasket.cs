using System.ComponentModel.DataAnnotations;

namespace FruitsBasket.Api.Basket.Contract;

public class CreateBasket
{
    [Length(2, 32)]
    public string Name { get; set; } = null!;

    [Range(0.001, 50)]
    public decimal FruitsWeight { get; set; }

    [DateRange("2000-01-01", "2026-01-01")]
    public DateTime LastFruitAdded { get; set; }
}