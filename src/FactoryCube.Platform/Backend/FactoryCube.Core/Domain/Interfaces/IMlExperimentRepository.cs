using FactoryCube.Core.Domain.Entities;

namespace FactoryCube.Core.Domain.Interfaces;

public interface IMlExperimentRepository : IRepository<MlExperiment>
{
    Task<IReadOnlyList<MlExperiment>> GetByProjectAsync(Guid projectId, CancellationToken ct = default);
    Task<IReadOnlyList<MlExperiment>> GetByDatasetAsync(Guid datasetId, CancellationToken ct = default);
    Task<MlExperiment?> GetByIdWithModelsAsync(Guid id, CancellationToken ct = default);
}
