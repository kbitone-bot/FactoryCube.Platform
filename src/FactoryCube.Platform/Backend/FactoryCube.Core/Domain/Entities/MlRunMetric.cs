namespace FactoryCube.Core.Domain.Entities;

public class MlRunMetric
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ExperimentId { get; set; }
    public MlExperiment Experiment { get; set; } = null!;
    public string RunId { get; set; } = string.Empty;
    public int? Epoch { get; set; }
    public string MetricName { get; set; } = string.Empty;
    public decimal MetricValue { get; set; }
    public int? Step { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
