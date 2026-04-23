using FactoryCube.Core.Domain.Entities;
using FactoryCube.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FactoryCube.Core.Infrastructure.Data.Repositories;

public class MlExperimentRepository : Repository<MlExperiment>, IMlExperimentRepository
{
    public MlExperimentRepository(FactoryCubeDbContext context) : base(context) { }

    public async Task<IReadOnlyList<MlExperiment>> GetByProjectAsync(Guid projectId, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(e => e.ProjectId == projectId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<MlExperiment>> GetByDatasetAsync(Guid datasetId, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(e => e.DatasetId == datasetId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<MlExperiment?> GetByIdWithModelsAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbSet
            .Include(e => e.ModelVersions)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }
}
