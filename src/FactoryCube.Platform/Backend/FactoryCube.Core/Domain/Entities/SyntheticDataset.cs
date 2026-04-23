namespace FactoryCube.Core.Domain.Entities;

public class SyntheticDataset
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid JobId { get; set; }
    public SyntheticJob Job { get; set; } = null!;
    public Guid DatasetId { get; set; }
    public Dataset Dataset { get; set; } = null!;
    public string GenerationMethod { get; set; } = string.Empty;
    public string? ScenarioMix { get; set; } // JSON
    public int? Seed { get; set; }
    public decimal? NoiseLevel { get; set; }
    public decimal? DriftLevel { get; set; }
    public List<string> LabelFields { get; set; } = new();
    public decimal? ValidationScore { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
