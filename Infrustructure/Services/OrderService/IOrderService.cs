using Infrustructure.DTOs;
using Infrustructure.Enums;

namespace Infrustructure.Services.OrderService;

public interface IOrderService
{
    Task<bool> CreateOrderAsync(CreateOrderDto order);
    Task<bool> UpdateOrderAsync(UpdateOrderDto order);
    Task<bool> DeleteOrderAsync(int orderId);
    Task<GetOrderDto?> GetOrderByIdAsync(int orderId);
    Task<IEnumerable<GetOrderDto>> GetAllOrdersAsync();
    
    public Task<IEnumerable<GetOrderBySupplierAndStatusDto>> GetOrdersBySupplierAndStatusAsync(int supplierId, Status status);
    public Task<IEnumerable<GetOrderDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
    public Task<IEnumerable<GetOrderDto>> GetOrdersWithPaginationAsync(int paheNumber, int pageSize);
}