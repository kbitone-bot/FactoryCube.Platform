namespace FactoryCube.Core.Domain.Entities;

public class RawRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DatasetId { get; set; }
    public Dataset Dataset { get; set; } = null!;
    public Guid? SourceFileId { get; set; }
    public DatasetFile? SourceFile { get; set; }
    public string RawData { get; set; } = "{}"; // JSON
    public string? RawHash { get; set; }
    public DateTime IngestionTime { get; set; } = DateTime.UtcNow;
    public long SeqNo { get; set; }
}
