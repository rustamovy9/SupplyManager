using Infrustructure.Enums;

namespace Infrustructure.DTOs;

public readonly record struct GetProductWithCategoryDto(
    int Id,
    string Name,
    string Description,
    decimal Price,
    int Quantity,
    int CategoryId,
    string CategoryName);
    
    
public readonly record struct GetOrderBySupplierAndStatusDto(
    int Id ,
    int ProductId,
    int Quantity,
    int SupplierId,
    Status Status,
    string SupplierName,
    DateTime OrderDate);
    

    
public readonly record struct CategoryWithProductCountDto(
    int CategoryId,
    string CategoryName,
    int ProductCount);
    
public readonly record struct GetProductDetailsDto(
    int ProductId,
    string ProductName,
    string ProductDescription,
    decimal ProductPrice,
    int ProductQuantity,
    string CategoryName,
    string SupplierName);