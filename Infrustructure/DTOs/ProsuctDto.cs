namespace Infrustructure.DTOs;

public readonly record struct GetProductDto(
    int Id,
    string Name,
    string Description,
    int Quantity,
    decimal Price,
    int CategoryId);
    
public readonly record struct CreateProductDto(
    string Name,
    string Description,
    int Quantity,
    decimal Price,
    int CategoryId);
    
public readonly record struct UpdateProductDto(
    int Id,
    string Name,
    string Description,
    int Quantity,
    decimal Price,
    int CategoryId);