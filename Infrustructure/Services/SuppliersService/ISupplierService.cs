using Infrustructure.DTOs;

namespace Infrustructure.Services.SuppliersService;

public interface ISupplierService
{
    Task<bool> CreateSupplierAsync(CreateSupplierDto supplier);
    Task<bool> UpdateSupplierAsync(UpdateSupplierDto supplier);
    Task<bool> DeleteSupplierAsync(int id);
    Task<GetSupplierDto?> GetSupplierByIdAsync(int id);
    Task<IEnumerable<GetSupplierDto>> GetAllSuppliersAsync();
}