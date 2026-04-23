using FactoryCube.Core.Domain.Entities;
using FactoryCube.Core.Domain.Interfaces;
using FactoryCube.Core.Infrastructure.PythonRunner;
using System.Text.Json;

namespace FactoryCube.Core.Infrastructure.Jobs;

public class SyntheticJobBackgroundService : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<SyntheticJobBackgroundService> _logger;

    public SyntheticJobBackgroundService(IServiceProvider sp, ILogger<SyntheticJobBackgroundService> logger)
    {
        _sp = sp;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SyntheticJobBackgroundService started.");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _sp.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<ISyntheticJobRepository>();
                var runner = scope.ServiceProvider.GetRequiredService<PythonRunnerService>();
                var dsRepo = scope.ServiceProvider.GetRequiredService<IDatasetRepository>();
                var jobs = await repo.GetPendingJobsAsync(stoppingToken);
                foreach (var job in jobs)
                {
                    await ProcessJobAsync(job, repo, runner, dsRepo, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SyntheticJobBackgroundService");
            }
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task ProcessJobAsync(
        SyntheticJob job,
        ISyntheticJobRepository repo,
        PythonRunnerService runner,
        IDatasetRepository dsRepo,
        CancellationToken ct)
    {
        await repo.UpdateStatusAsync(job.Id, JobStatus.Running, 10, ct);
        var outputDir = Path.Combine("data", "synthetic", job.Id.ToString());
        var config = new Dictionary<string, object>
        {
            ["start_time"] = job.StartTime.ToString("O"),
            ["end_time"] = job.EndTime.ToString("O"),
            ["equipment_count"] = job.EquipmentCount,
            ["scenario_config"] = JsonSerializer.Deserialize<Dictionary<string, object>>(job.ScenarioConfig) ?? new()
        };

        var (success, output, error) = await runner.RunAsync("synthetic", config, string.Empty, outputDir, ct);
        if (!success)
        {
            job.Status = JobStatus.Failed;
            job.ErrorMessage = error;
            await repo.UpdateStatusAsync(job.Id, JobStatus.Failed, 0, ct);
            return;
        }

        // 결과 Dataset 생성
        var resultDataset = new Dataset
        {
            ProjectId = job.ProjectId,
            Name = $"Synthetic_{job.JobName}",
            SourceType = SourceType.Synthetic,
            Status = DatasetStatus.Ready
        };
        await dsRepo.AddAsync(resultDataset, ct);
        await dsRepo.SaveChangesAsync(ct);

        job.OutputDatasetId = resultDataset.Id;
        await repo.UpdateStatusAsync(job.Id, JobStatus.Completed, 100, ct);
    }
}
