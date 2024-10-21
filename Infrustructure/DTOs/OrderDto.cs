using Infrustructure.Enums;

namespace Infrustructure.DTOs;

public readonly record struct CreateOrderDto(
    int ProductId,
    int Quantity,
    int SupplierId);

public readonly record struct UpdateOrderDto(
    int Id,
    int ProductId,
    int Quantity,
    int SupplierId);


public readonly record struct GetOrderDto(
    int Id ,
    int ProductId,
    int Quantity,
    int SupplierId,
    Status Status,
    DateTime OrderDate);
