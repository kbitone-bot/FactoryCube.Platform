namespace FactoryCube.Core.Domain.Entities;

public class MlExperiment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string ExperimentName { get; set; } = string.Empty;
    public TaskType TaskType { get; set; } = TaskType.Classification;
    public Guid DatasetId { get; set; }
    public Dataset Dataset { get; set; } = null!;
    public string ModelType { get; set; } = string.Empty;
    public string? FeatureConfig { get; set; } // JSON
    public string? Hyperparameters { get; set; } // JSON
    public string? TrainConfig { get; set; } // JSON
    public ExperimentStatus Status { get; set; } = ExperimentStatus.Draft;
    public string? BestModelPath { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<MlModelRegistry> ModelVersions { get; set; } = new List<MlModelRegistry>();
    public ICollection<MlRunMetric> Metrics { get; set; } = new List<MlRunMetric>();
    public ICollection<PredictionResult> Predictions { get; set; } = new List<PredictionResult>();
}

public enum TaskType { Classification, Regression, AnomalyDetection, TimeSeries }
public enum ExperimentStatus { Draft, Running, Completed, Failed }
