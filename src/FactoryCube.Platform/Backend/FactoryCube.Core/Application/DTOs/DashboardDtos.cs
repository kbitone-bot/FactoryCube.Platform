namespace FactoryCube.Core.Application.DTOs;

public record DashboardKpiDto(
    string Label,
    decimal Value,
    string? Unit,
    decimal? ChangePct,
    string? Trend
);

public record DashboardSnapshotDto(
    Guid Id,
    Guid ProjectId,
    DateTime SnapshotTime,
    string SnapshotType,
    List<DashboardKpiDto> Kpis,
    object? ChartData
);

public record EquipmentStatusSummaryDto(
    string EquipmentId,
    string CurrentState,
    decimal AvailabilityPct,
    decimal HealthScore,
    DateTime LastSeen
);
