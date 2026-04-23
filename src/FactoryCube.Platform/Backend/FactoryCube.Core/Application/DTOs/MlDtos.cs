namespace FactoryCube.Core.Application.DTOs;

public record CreateExperimentRequest(
    Guid ProjectId,
    string ExperimentName,
    string TaskType,
    Guid DatasetId,
    string ModelType,
    Dictionary<string, object>? FeatureConfig,
    Dictionary<string, object>? Hyperparameters,
    Dictionary<string, object>? TrainConfig
);

public record ExperimentDto(
    Guid Id,
    Guid ProjectId,
    string ExperimentName,
    string TaskType,
    Guid DatasetId,
    string ModelType,
    string Status,
    string? BestModelPath,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record ModelRegistryDto(
    Guid Id,
    Guid ExperimentId,
    int ModelVersion,
    string ModelPath,
    bool IsDeployed,
    DateTime? DeployedAt,
    DateTime CreatedAt
);

public record PredictionDto(
    Guid Id,
    Guid ExperimentId,
    string EquipmentId,
    DateTime PredictionTime,
    string? PredictedClass,
    decimal? PredictedValue,
    decimal? Probability,
    decimal? AnomalyScore
);

public record TrainJobRequest(
    Guid ExperimentId
);

public record InferenceJobRequest(
    Guid ExperimentId,
    Guid DatasetId
);
