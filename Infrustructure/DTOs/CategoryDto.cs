namespace Infrustructure.DTOs;

public readonly record struct CreateCategoryDto(
    string Name,
    string Description);

public readonly record struct UpdateCategoryDto(
    int Id,
    string Name,
    string Description);

public readonly record struct GetCategoryDto(
    int Id,
    string Name,
    string Description);
