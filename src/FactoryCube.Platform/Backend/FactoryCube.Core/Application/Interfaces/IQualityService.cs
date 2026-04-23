using FactoryCube.Core.Application.DTOs;

namespace FactoryCube.Core.Application.Interfaces;

public interface IQualityService
{
    Task<QualityRuleDto> CreateRuleAsync(CreateQualityRuleRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<QualityRuleDto>> GetRulesByProjectAsync(Guid? projectId, CancellationToken ct = default);
    Task<QualitySummaryDto> RunQualityCheckAsync(Guid datasetId, CancellationToken ct = default);
    Task<QualitySummaryDto?> GetLatestResultAsync(Guid datasetId, CancellationToken ct = default);
}
