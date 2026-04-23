namespace FactoryCube.Core.Domain.Entities;

public class PredictionResult
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ExperimentId { get; set; }
    public MlExperiment Experiment { get; set; } = null!;
    public Guid? ModelRegistryId { get; set; }
    public MlModelRegistry? ModelRegistry { get; set; }
    public Guid DatasetId { get; set; }
    public Dataset Dataset { get; set; } = null!;
    public string EquipmentId { get; set; } = string.Empty;
    public DateTime PredictionTime { get; set; }
    public string? TargetField { get; set; }
    public decimal? PredictedValue { get; set; }
    public string? PredictedClass { get; set; }
    public decimal? Probability { get; set; }
    public decimal? AnomalyScore { get; set; }
    public string? FeatureSnapshot { get; set; } // JSON
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
