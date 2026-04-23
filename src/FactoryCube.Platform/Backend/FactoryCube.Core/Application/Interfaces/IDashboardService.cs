using FactoryCube.Core.Application.DTOs;

namespace FactoryCube.Core.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardSnapshotDto> GetLatestSnapshotAsync(Guid projectId, CancellationToken ct = default);
    Task<IReadOnlyList<EquipmentStatusSummaryDto>> GetEquipmentStatusAsync(Guid projectId, CancellationToken ct = default);
}
