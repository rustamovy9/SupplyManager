using Infrustructure.DTOs;
using Infrustructure.Entities;
using Infrustructure.Services.CategoryService;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Controllers;


[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        this._categoryService = categoryService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCategoriesAsync()
    {
        return Ok(await _categoryService.GetAllCategoriesAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCategoryByIdAsync(int id)
    {
        GetCategoryDto? category = await _categoryService.GetCategoryByIdAsync(id)!;
        return category is null ? NotFound("Category not found") : Ok(category);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryDto categories)
    {
        bool isCreated = await _categoryService.CreateCategoryAsync(categories);
        return isCreated is false ? BadRequest("Failed to create category") : Ok(isCreated);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCategoryAsync([FromBody] UpdateCategoryDto categories)
    {
        bool isUpdate = await _categoryService.UpdateCategoryAsync(categories);
        return isUpdate is false ? NotFound("Category not found") : Ok(isUpdate);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCategoryAsync(int id)
    {
        bool isDelete = await _categoryService.DeleteCategoryAsync(id);
        return isDelete is false ? NotFound("Category not found") : Ok(isDelete);
    }

    [HttpGet("withProductCount")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCategoriesWithProductCountAsync()
    {
        return Ok(await _categoryService.GetCategoriesWithProductCountAsync());
    }
}

