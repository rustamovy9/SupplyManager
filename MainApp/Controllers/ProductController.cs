using Infrustructure.DTOs;
using Infrustructure.Entities;
using Infrustructure.Services.CategoryService;
using Infrustructure.Services.ProductService;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Controllers;


[ApiController]
[Route("api/products")]
public class ProductController:ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        this._productService = productService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProductsAsync()
    {
        return Ok(await _productService.GetAllProductsAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProductByIdAsync(int id)
    {
        GetProductDto? product = await _productService.GetProductByIdAsync(id)!;
        return product is null? NotFound("Category not found") : Ok(product);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductDto product)
    {
        bool isCreated = await _productService.CreateProductAsync(product);
        return isCreated is false? BadRequest("Failed to create prosuct") : Ok(isCreated);
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProductAsync([FromBody] UpdateProductDto product)
    {
        bool isUpdate = await _productService.UpdateProductAsync(product);
        return isUpdate is false? NotFound("Product not found") : Ok(isUpdate);  
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProductAsync(int id)
    {
        bool isDelete = await _productService.DeleteProductAsync(id);
        return isDelete is false? NotFound("Category not found") : Ok(isDelete);
    }
    
    [HttpGet("{categoryId:int}/sortBy=price/asc=true|desc=false{sortedPrice:bool}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProductByFilterCategoryAndOrderByPriceAsync(int categoryId,bool sortedPrice)
    {
        IEnumerable<GetProductWithCategoryDto>? products = await _productService.GetProductByFilterCategoryAndOrderByPrice(categoryId,sortedPrice);
        return products is List<GetProductWithCategoryDto> ? NotFound("No products found for this category") : Ok(products);
    }

    [HttpGet("maxQuantity={maxQuantity:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProductByMaxQuantityAsync(int maxQuantity)
    {
        IEnumerable<GetProductDto>? products = await _productService.GetProductsByMaxQuantityAsync(maxQuantity);
        return products is List<GetProductDto> ? NotFound("No products found with quantity less than or equal to the given maximum") : Ok(products);
    }
    [HttpGet("{id}/details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProductDetailsAsync(int id)
    {
        GetProductDetailsDto? product = await _productService.GetProductDetailsAsync(id)!;
        return product is null? NotFound("Product not found") : Ok(product);
    }
    

}