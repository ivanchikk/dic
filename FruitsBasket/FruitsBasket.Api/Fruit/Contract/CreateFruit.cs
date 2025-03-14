using System.ComponentModel.DataAnnotations;

namespace FruitsBasket.Api.Fruit.Contract;

public class CreateFruit
{
    [Length(2, 32)]
    public string Name { get; set; } = null!;
    
    [Range(0.001, 50)]
    public decimal Weight { get; set; }
    
    [Range(typeof(DateTime), "2000-01-01", "2026-01-01")]
    public DateTime HarvestDate { get; set; }
}