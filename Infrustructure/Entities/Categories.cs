namespace Infrustructure.Entities;

public class Categories:BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ICollection<Products> Products { get; set; } = [];
}