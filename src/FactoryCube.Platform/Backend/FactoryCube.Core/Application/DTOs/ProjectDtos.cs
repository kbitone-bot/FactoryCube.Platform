namespace FactoryCube.Core.Application.DTOs;

public record CreateProjectRequest(
    string Name,
    string? Description,
    string EquipmentType
);

public record ProjectDto(
    Guid Id,
    string Name,
    string? Description,
    string EquipmentType,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string CreatedBy
);

public record UpdateProjectRequest(
    string Name,
    string? Description,
    string Status
);
