using System.ComponentModel.DataAnnotations;
using FruitsBasket.Model;

namespace FruitsBasket.Data.Basket;

public class BasketDao : ISoftDeletable
{
    [Key]
    public Guid Id { get; set; }
    [MaxLength(32)]
    public string Name { get; set; } = null!;
    public decimal FruitsWeight { get; set; }
    public DateTime LastFruitAdded { get; set; }
    public bool IsDeleted { get; set; }
}