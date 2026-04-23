using FactoryCube.Core.Application.DTOs;

namespace FactoryCube.Core.Application.Interfaces;

public interface IMlService
{
    Task<ExperimentDto> CreateExperimentAsync(CreateExperimentRequest request, CancellationToken ct = default);
    Task<ExperimentDto?> GetExperimentByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ExperimentDto>> GetExperimentsByProjectAsync(Guid projectId, CancellationToken ct = default);
    Task<bool> StartTrainingAsync(Guid experimentId, CancellationToken ct = default);
    Task<bool> RunInferenceAsync(Guid experimentId, Guid datasetId, CancellationToken ct = default);
    Task<IReadOnlyList<PredictionDto>> GetPredictionsAsync(Guid experimentId, CancellationToken ct = default);
}
