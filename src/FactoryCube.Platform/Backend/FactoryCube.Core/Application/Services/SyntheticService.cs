using FactoryCube.Core.Application.DTOs;
using FactoryCube.Core.Application.Interfaces;
using FactoryCube.Core.Domain.Entities;
using FactoryCube.Core.Domain.Interfaces;

namespace FactoryCube.Core.Application.Services;

public class SyntheticService : ISyntheticService
{
    private readonly ISyntheticJobRepository _repo;
    private readonly IRepository<SyntheticDataset> _dsRepo;
    private readonly IRepository<SyntheticValidation> _valRepo;

    public SyntheticService(
        ISyntheticJobRepository repo,
        IRepository<SyntheticDataset> dsRepo,
        IRepository<SyntheticValidation> valRepo)
    {
        _repo = repo;
        _dsRepo = dsRepo;
        _valRepo = valRepo;
    }

    public async Task<SyntheticJobDto> CreateJobAsync(CreateSyntheticJobRequest request, CancellationToken ct = default)
    {
        var job = new SyntheticJob
        {
            ProjectId = request.ProjectId,
            JobName = request.JobName,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            EquipmentCount = request.EquipmentCount,
            ScenarioConfig = System.Text.Json.JsonSerializer.Serialize(request.ScenarioConfig)
        };
        await _repo.AddAsync(job, ct);
        await _repo.SaveChangesAsync(ct);
        return MapToDto(job);
    }

    public async Task<SyntheticJobDto?> GetJobByIdAsync(Guid id, CancellationToken ct = default)
    {
        var job = await _repo.GetByIdAsync(id, ct);
        return job == null ? null : MapToDto(job);
    }

    public async Task<IReadOnlyList<SyntheticJobDto>> GetJobsByProjectAsync(Guid projectId, CancellationToken ct = default)
    {
        var jobs = await _repo.GetByProjectAsync(projectId, ct);
        return jobs.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<SyntheticValidationDto>> GetValidationsAsync(Guid syntheticDatasetId, CancellationToken ct = default)
    {
        var vals = await _valRepo.GetAllAsync(ct); // 실제로는 필터 필요
        return vals.Where(v => v.SyntheticDatasetId == syntheticDatasetId)
            .Select(v => new SyntheticValidationDto(
                v.Id, v.MetricName, v.OriginalValue, v.SyntheticValue,
                v.DifferencePct, v.IsPassed))
            .ToList();
    }

    private static SyntheticJobDto MapToDto(SyntheticJob j) => new(
        j.Id, j.ProjectId, j.JobName, j.StartTime, j.EndTime,
        j.EquipmentCount, j.Status.ToString(), j.ProgressPct,
        j.OutputDatasetId, j.CreatedAt, j.CompletedAt);
}
