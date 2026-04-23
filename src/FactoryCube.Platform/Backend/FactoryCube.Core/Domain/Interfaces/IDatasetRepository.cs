using FactoryCube.Core.Domain.Entities;

namespace FactoryCube.Core.Domain.Interfaces;

public interface IDatasetRepository : IRepository<Dataset>
{
    Task<IReadOnlyList<Dataset>> GetByProjectAsync(Guid projectId, CancellationToken ct = default);
    Task<Dataset?> GetByIdWithFilesAsync(Guid id, CancellationToken ct = default);
    Task UpdateStatusAsync(Guid id, DatasetStatus status, CancellationToken ct = default);
}
