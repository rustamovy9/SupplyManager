using Infrustructure.Enums;

namespace Infrustructure.Entities;

public class Orders:BaseEntity
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int SupplierId { get; set; }
    public Status Status { get; set; }
    public Products Products { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public Suppliers Suppliers { get; set; }=null!;
}
