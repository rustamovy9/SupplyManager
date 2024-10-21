namespace Infrustructure.DTOs;

public readonly record struct CreateSupplierDto(
    string Name,
    string ContactPerson,
    string Email,
    string Phone);
    
public readonly record struct UpdateSupplierDto(
    int Id,
    string Name,
    string ContactPerson,
    string Email,
    string Phone);
    
public readonly record struct GetSupplierDto(
    int Id,
    string Name,
    string ContactPerson,
    string Email,
    string Phone);