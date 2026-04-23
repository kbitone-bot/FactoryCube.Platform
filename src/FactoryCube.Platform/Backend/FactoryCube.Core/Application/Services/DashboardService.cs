using FactoryCube.Core.Application.DTOs;
using FactoryCube.Core.Application.Interfaces;
using FactoryCube.Core.Domain.Entities;
using FactoryCube.Core.Domain.Interfaces;

namespace FactoryCube.Core.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IRepository<DashboardSnapshot> _snapRepo;
    private readonly IRepository<PredictionResult> _predRepo;

    public DashboardService(IRepository<DashboardSnapshot> snapRepo, IRepository<PredictionResult> predRepo)
    {
        _snapRepo = snapRepo;
        _predRepo = predRepo;
    }

    public async Task<DashboardSnapshotDto> GetLatestSnapshotAsync(Guid projectId, CancellationToken ct = default)
    {
        var snaps = await _snapRepo.GetAllAsync(ct);
        var latest = snaps.Where(s => s.ProjectId == projectId).OrderByDescending(s => s.SnapshotTime).FirstOrDefault();
        if (latest == null)
        {
            return new DashboardSnapshotDto(Guid.Empty, projectId, DateTime.UtcNow, "default",
                new List<DashboardKpiDto>(), null);
        }
        return new DashboardSnapshotDto(latest.Id, latest.ProjectId, latest.SnapshotTime, latest.SnapshotType,
            new List<DashboardKpiDto>(), null);
    }

    public async Task<IReadOnlyList<EquipmentStatusSummaryDto>> GetEquipmentStatusAsync(Guid projectId, CancellationToken ct = default)
    {
        // 실제 구현에서는 fc_timeseries_1s/1m 테이블에서 집계
        return new List<EquipmentStatusSummaryDto>
        {
            new("EQ001", "RUNNING", 92.5m, 87.0m, DateTime.UtcNow),
            new("EQ002", "IDLE", 45.0m, 72.0m, DateTime.UtcNow),
        };
    }
}
