namespace FruitsBasket.Model.Basket;

public class BasketDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal FruitsWeight { get; set; }
    public DateTime LastFruitAdded { get; set; }
}