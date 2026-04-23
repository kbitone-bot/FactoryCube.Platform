using FactoryCube.Core.Domain.Entities;

namespace FactoryCube.Core.Domain.Interfaces;

public interface ISyntheticJobRepository : IRepository<SyntheticJob>
{
    Task<IReadOnlyList<SyntheticJob>> GetPendingJobsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<SyntheticJob>> GetByProjectAsync(Guid projectId, CancellationToken ct = default);
    Task UpdateStatusAsync(Guid id, JobStatus status, int progressPct, CancellationToken ct = default);
}
