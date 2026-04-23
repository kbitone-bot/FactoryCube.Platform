using FactoryCube.Core.Application.DTOs;
using FactoryCube.Core.Application.Interfaces;
using FactoryCube.Core.Domain.Entities;
using FactoryCube.Core.Domain.Interfaces;
using FactoryCube.Core.Infrastructure.PythonRunner;
using System.Text.Json;

namespace FactoryCube.Core.Application.Services;

public class MlService : IMlService
{
    private readonly IMlExperimentRepository _repo;
    private readonly PythonRunnerService _runner;

    public MlService(IMlExperimentRepository repo, PythonRunnerService runner)
    {
        _repo = repo;
        _runner = runner;
    }

    public async Task<ExperimentDto> CreateExperimentAsync(CreateExperimentRequest request, CancellationToken ct = default)
    {
        Enum.TryParse<TaskType>(request.TaskType, true, out var taskType);
        var exp = new MlExperiment
        {
            ProjectId = request.ProjectId,
            ExperimentName = request.ExperimentName,
            TaskType = taskType,
            DatasetId = request.DatasetId,
            ModelType = request.ModelType,
            FeatureConfig = request.FeatureConfig != null ? JsonSerializer.Serialize(request.FeatureConfig) : null,
            Hyperparameters = request.Hyperparameters != null ? JsonSerializer.Serialize(request.Hyperparameters) : null,
            TrainConfig = request.TrainConfig != null ? JsonSerializer.Serialize(request.TrainConfig) : null,
        };
        await _repo.AddAsync(exp, ct);
        await _repo.SaveChangesAsync(ct);
        return MapToDto(exp);
    }

    public async Task<ExperimentDto?> GetExperimentByIdAsync(Guid id, CancellationToken ct = default)
    {
        var exp = await _repo.GetByIdAsync(id, ct);
        return exp == null ? null : MapToDto(exp);
    }

    public async Task<IReadOnlyList<ExperimentDto>> GetExperimentsByProjectAsync(Guid projectId, CancellationToken ct = default)
    {
        var list = await _repo.GetByProjectAsync(projectId, ct);
        return list.Select(MapToDto).ToList();
    }

    public async Task<bool> StartTrainingAsync(Guid experimentId, CancellationToken ct = default)
    {
        var exp = await _repo.GetByIdAsync(experimentId, ct);
        if (exp == null) return false;
        // 실제 구현에서는 Background Job으로 위임
        return true;
    }

    public async Task<bool> RunInferenceAsync(Guid experimentId, Guid datasetId, CancellationToken ct = default)
    {
        var exp = await _repo.GetByIdAsync(experimentId, ct);
        if (exp == null) return false;
        return true;
    }

    public async Task<IReadOnlyList<PredictionDto>> GetPredictionsAsync(Guid experimentId, CancellationToken ct = default)
    {
        return new List<PredictionDto>();
    }

    private static ExperimentDto MapToDto(MlExperiment e) => new(
        e.Id, e.ProjectId, e.ExperimentName, e.TaskType.ToString(),
        e.DatasetId, e.ModelType, e.Status.ToString(), e.BestModelPath,
        e.CreatedAt, e.UpdatedAt);
}
