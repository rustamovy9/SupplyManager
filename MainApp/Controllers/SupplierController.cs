using Infrustructure.DTOs;
using Infrustructure.Services.SuppliersService;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Controllers;

[ApiController]
[Route("api/suppliers")]
public class SupplierController:ControllerBase
{
     private readonly ISupplierService _supplierService;

    public SupplierController(ISupplierService supplierService)
    {
        this._supplierService = supplierService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSuppliersAsync()
    {
        return Ok(await _supplierService.GetAllSuppliersAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSupplierByIdAsync(int id)
    {
        GetSupplierDto? supplier = await _supplierService.GetSupplierByIdAsync(id)!;
        return supplier is null? NotFound("Supplier not found") : Ok(supplier);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateSupplierAsync([FromBody] CreateSupplierDto supplier)
    {
        bool isCreated = await _supplierService.CreateSupplierAsync(supplier);
        return isCreated is false? BadRequest("Failed to create supplier") : Ok(isCreated);
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateSupplierAsync([FromBody] UpdateSupplierDto supplier)
    {
        bool isUpdate = await _supplierService.UpdateSupplierAsync(supplier);
        return isUpdate is false? NotFound("Supplier not found") : Ok(isUpdate);  
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteSupplierAsync(int id)
    {
        bool isDelete = await _supplierService.DeleteSupplierAsync(id);
        return isDelete is false? NotFound("Supplier not found") : Ok(isDelete);
    }

}