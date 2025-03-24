using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FruitsBasket.Model.Fruit;

namespace FruitsBasket.Data.Fruit;

[Table("fruits")]
public class FruitDao : ISoftDeletable
{
    [Column("id"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("weight")]
    public decimal Weight { get; set; }

    [Column("harvest_date")] 
    public DateTime HarvestDate { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }
}