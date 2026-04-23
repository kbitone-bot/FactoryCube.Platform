namespace FactoryCube.Core.Domain.Entities;

public class MlModelRegistry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ExperimentId { get; set; }
    public MlExperiment Experiment { get; set; } = null!;
    public int ModelVersion { get; set; }
    public string ModelPath { get; set; } = string.Empty;
    public string? Metrics { get; set; } // JSON
    public bool IsDeployed { get; set; }
    public DateTime? DeployedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
