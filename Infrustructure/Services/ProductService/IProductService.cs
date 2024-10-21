using Infrustructure.DTOs;
using Infrustructure.Entities;

namespace Infrustructure.Services.ProductService;

public interface IProductService
{
    Task<bool> CreateProductAsync(CreateProductDto products);
    Task<bool> UpdateProductAsync(UpdateProductDto products);
    Task<bool> DeleteProductAsync(int id);
    Task<GetProductDto?> GetProductByIdAsync(int id);
    Task<IEnumerable<GetProductDto>> GetAllProductsAsync();
    Task<IEnumerable<GetProductWithCategoryDto>> GetProductByFilterCategoryAndOrderByPrice(int? categoryId, bool sortByAsc = true);
    Task<IEnumerable<GetProductDto>> GetProductsByMaxQuantityAsync(int maxQuantityId);
    Task<GetProductDetailsDto?> GetProductDetailsAsync(int productId);
}