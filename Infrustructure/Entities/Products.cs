namespace Infrustructure.Entities;

public class Products:BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; }=null!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public Categories Category { get; set; } = null!;
}