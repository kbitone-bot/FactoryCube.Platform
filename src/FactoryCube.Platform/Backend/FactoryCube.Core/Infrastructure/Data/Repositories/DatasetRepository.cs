using FactoryCube.Core.Domain.Entities;
using FactoryCube.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FactoryCube.Core.Infrastructure.Data.Repositories;

public class DatasetRepository : Repository<Dataset>, IDatasetRepository
{
    public DatasetRepository(FactoryCubeDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Dataset>> GetByProjectAsync(Guid projectId, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(d => d.ProjectId == projectId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<Dataset?> GetByIdWithFilesAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbSet
            .Include(d => d.Files)
            .FirstOrDefaultAsync(d => d.Id == id, ct);
    }

    public async Task UpdateStatusAsync(Guid id, DatasetStatus status, CancellationToken ct = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, ct);
        if (entity != null)
        {
            entity.Status = status;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
        }
    }
}
