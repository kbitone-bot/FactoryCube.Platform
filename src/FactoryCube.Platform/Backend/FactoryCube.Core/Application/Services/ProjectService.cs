using FactoryCube.Core.Application.DTOs;
using FactoryCube.Core.Application.Interfaces;
using FactoryCube.Core.Domain.Entities;
using FactoryCube.Core.Domain.Interfaces;

namespace FactoryCube.Core.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _repo;

    public ProjectService(IProjectRepository repo)
    {
        _repo = repo;
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectRequest request, string createdBy, CancellationToken ct = default)
    {
        Enum.TryParse<EquipmentType>(request.EquipmentType, true, out var eqType);
        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            EquipmentType = eqType,
            CreatedBy = createdBy
        };
        await _repo.AddAsync(project, ct);
        await _repo.SaveChangesAsync(ct);
        return MapToDto(project);
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var project = await _repo.GetByIdAsync(id, ct);
        return project == null ? null : MapToDto(project);
    }

    public async Task<IReadOnlyList<ProjectDto>> GetAllAsync(CancellationToken ct = default)
    {
        var projects = await _repo.GetAllAsync(ct);
        return projects.Select(MapToDto).ToList();
    }

    public async Task<ProjectDto?> UpdateAsync(Guid id, UpdateProjectRequest request, CancellationToken ct = default)
    {
        var project = await _repo.GetByIdAsync(id, ct);
        if (project == null) return null;
        project.Name = request.Name;
        project.Description = request.Description;
        Enum.TryParse<ProjectStatus>(request.Status, true, out var status);
        project.Status = status;
        project.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(project, ct);
        await _repo.SaveChangesAsync(ct);
        return MapToDto(project);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var project = await _repo.GetByIdAsync(id, ct);
        if (project == null) return false;
        project.Status = ProjectStatus.Deleted;
        await _repo.UpdateAsync(project, ct);
        await _repo.SaveChangesAsync(ct);
        return true;
    }

    private static ProjectDto MapToDto(Project p) => new(
        p.Id, p.Name, p.Description, p.EquipmentType.ToString(),
        p.Status.ToString(), p.CreatedAt, p.UpdatedAt, p.CreatedBy);
}
