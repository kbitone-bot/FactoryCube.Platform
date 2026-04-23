namespace FactoryCube.Core.Domain.Entities;

public class NormalizedRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DatasetId { get; set; }
    public Dataset Dataset { get; set; } = null!;
    public DateTime RecordTime { get; set; }
    public string EquipmentId { get; set; } = string.Empty;
    public string SourceType { get; set; } = string.Empty;
    public string TagValues { get; set; } = "{}"; // JSON
    public string? State { get; set; }
    public string QualityFlags { get; set; } = "[]"; // JSON array
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
