namespace FactoryCube.Core.Domain.Entities;

public class SyntheticValidation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SyntheticDatasetId { get; set; }
    public SyntheticDataset SyntheticDataset { get; set; } = null!;
    public Guid? OriginalDatasetId { get; set; }
    public Dataset? OriginalDataset { get; set; }
    public string MetricName { get; set; } = string.Empty;
    public decimal? OriginalValue { get; set; }
    public decimal? SyntheticValue { get; set; }
    public decimal? DifferencePct { get; set; }
    public decimal? PassThreshold { get; set; }
    public bool IsPassed { get; set; }
    public string? Details { get; set; } // JSON
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
