namespace Infrustructure.Entities;

public class Suppliers:BaseEntity
{
    public string Name { get; set; } = null!;
    public string ContactPerson { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public ICollection<Orders> Orders { get; set; } = [];
}