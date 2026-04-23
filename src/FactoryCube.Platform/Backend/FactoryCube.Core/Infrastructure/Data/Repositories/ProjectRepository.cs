using FactoryCube.Core.Domain.Entities;
using FactoryCube.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FactoryCube.Core.Infrastructure.Data.Repositories;

public class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(FactoryCubeDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Project>> GetByEquipmentTypeAsync(string equipmentType, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(p => p.EquipmentType.ToString() == equipmentType)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<Project?> GetByIdWithDatasetsAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbSet
            .Include(p => p.Datasets)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }
}
