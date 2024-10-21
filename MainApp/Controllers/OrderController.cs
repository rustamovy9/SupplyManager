using Infrustructure.DTOs;
using Infrustructure.Enums;
using Infrustructure.Services.OrderService;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController:ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        this._orderService = orderService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOrdersAsync()
    {
        return Ok(await _orderService.GetAllOrdersAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOrderByIdAsync(int id)
    {
        GetOrderDto? order = await _orderService.GetOrderByIdAsync(id)!;
        return order is null? NotFound("Order not found") : Ok(order);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOrderAsync([FromBody] CreateOrderDto order)
    {
        bool isCreated = await _orderService.CreateOrderAsync(order);
        return isCreated is false? BadRequest("Failed to create order") : Ok(isCreated);
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateOrderAsync([FromBody] UpdateOrderDto order)
    {
        bool isUpdate = await _orderService.UpdateOrderAsync(order);
        return isUpdate is false? NotFound("Order not found") : Ok(isUpdate);  
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteOrderAsync(int id)
    {
        bool isDelete = await _orderService.DeleteOrderAsync(id);
        return isDelete is false? NotFound("Order not found") : Ok(isDelete);
    }


    [HttpGet("supplierId={supplierId:int}&status={status}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOrderBySupplierIdAndStatusAsync(int supplierId, Status status)
    {
        IEnumerable<GetOrderBySupplierAndStatusDto> order = await _orderService.GetOrdersBySupplierAndStatusAsync(supplierId, status)!;
        return order is null? NotFound("Order not found") : Ok(order);
    }

    [HttpGet("startDate={startDate:datetime}&endDate={endDate:datetime}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOrdersByDateAsync(DateTime startDate, DateTime endDate)
    {
        IEnumerable<GetOrderDto> order = await _orderService.GetOrdersByDateRangeAsync(startDate, endDate)!;
        return order is null ? NotFound("Order not found") : Ok(order);
    }
    
    [HttpGet("pageNumber={pageNumber:int}&pageSize={pageSize:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOrdersWithPaginationAsync(int pageNumber, int pageSize)
    {
        IEnumerable<GetOrderDto> order = await _orderService.GetOrdersWithPaginationAsync(pageNumber, pageSize)!;
        return order is null? NotFound("Order not found") : Ok(order);
    }

}