namespace FruitsBasket.Model.Fruit;

public class FruitDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Weight { get; set; }
    public DateTime HarvestDate { get; set; }
}