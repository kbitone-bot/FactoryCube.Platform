namespace FactoryCube.Core.Application.DTOs;

public record CreateSyntheticJobRequest(
    Guid ProjectId,
    string JobName,
    DateTime StartTime,
    DateTime EndTime,
    int EquipmentCount,
    SyntheticScenarioConfig ScenarioConfig
);

public record SyntheticScenarioConfig(
    List<string> Scenarios,
    decimal? NoiseLevel,
    decimal? DriftLevel,
    int? Seed,
    decimal? AnomalyRatio,
    string? EquipmentType
);

public record SyntheticJobDto(
    Guid Id,
    Guid ProjectId,
    string JobName,
    DateTime StartTime,
    DateTime EndTime,
    int EquipmentCount,
    string Status,
    int ProgressPct,
    Guid? OutputDatasetId,
    DateTime CreatedAt,
    DateTime? CompletedAt
);

public record SyntheticValidationDto(
    Guid Id,
    string MetricName,
    decimal? OriginalValue,
    decimal? SyntheticValue,
    decimal? DifferencePct,
    bool IsPassed
);
