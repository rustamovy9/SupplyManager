using Infrustructure.DTOs;
using Infrustructure.Entities;

namespace Infrustructure.Services.CategoryService;

public interface ICategoryService
{
    Task<bool> CreateCategoryAsync(CreateCategoryDto categories);
    Task<bool> UpdateCategoryAsync(UpdateCategoryDto categories);
    Task<bool> DeleteCategoryAsync(int id);
    Task<GetCategoryDto?> GetCategoryByIdAsync(int id);
    Task<IEnumerable<GetCategoryDto>> GetAllCategoriesAsync();
    Task<IEnumerable<CategoryWithProductCountDto>> GetCategoriesWithProductCountAsync();
}