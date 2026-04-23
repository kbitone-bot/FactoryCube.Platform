using FactoryCube.Core.Application.DTOs;

namespace FactoryCube.Core.Application.Interfaces;

public interface ISyntheticService
{
    Task<SyntheticJobDto> CreateJobAsync(CreateSyntheticJobRequest request, CancellationToken ct = default);
    Task<SyntheticJobDto?> GetJobByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<SyntheticJobDto>> GetJobsByProjectAsync(Guid projectId, CancellationToken ct = default);
    Task<IReadOnlyList<SyntheticValidationDto>> GetValidationsAsync(Guid syntheticDatasetId, CancellationToken ct = default);
}
