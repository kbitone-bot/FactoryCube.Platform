using FactoryCube.Core.Domain.Entities;
using FactoryCube.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FactoryCube.Core.Infrastructure.Data.Repositories;

public class SyntheticJobRepository : Repository<SyntheticJob>, ISyntheticJobRepository
{
    public SyntheticJobRepository(FactoryCubeDbContext context) : base(context) { }

    public async Task<IReadOnlyList<SyntheticJob>> GetPendingJobsAsync(CancellationToken ct = default)
    {
        return await _dbSet
            .Where(j => j.Status == JobStatus.Pending)
            .OrderBy(j => j.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<SyntheticJob>> GetByProjectAsync(Guid projectId, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(j => j.ProjectId == projectId)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task UpdateStatusAsync(Guid id, JobStatus status, int progressPct, CancellationToken ct = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, ct);
        if (entity != null)
        {
            entity.Status = status;
            entity.ProgressPct = progressPct;
            if (status == JobStatus.Completed || status == JobStatus.Failed)
                entity.CompletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
        }
    }
}
