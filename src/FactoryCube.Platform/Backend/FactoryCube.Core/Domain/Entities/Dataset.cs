namespace FactoryCube.Core.Domain.Entities;

public class Dataset
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public SourceType SourceType { get; set; } = SourceType.Upload;
    public long RecordCount { get; set; }
    public DateTime? TimeRangeStart { get; set; }
    public DateTime? TimeRangeEnd { get; set; }
    public string? SchemaDetected { get; set; }
    public decimal? QualityScore { get; set; }
    public DatasetStatus Status { get; set; } = DatasetStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<DatasetFile> Files { get; set; } = new List<DatasetFile>();
    public ICollection<RawRecord> RawRecords { get; set; } = new List<RawRecord>();
    public ICollection<NormalizedRecord> NormalizedRecords { get; set; } = new List<NormalizedRecord>();
    public ICollection<QualityResult> QualityResults { get; set; } = new List<QualityResult>();
}

public enum SourceType { Upload, Watcher, Api, Synthetic }
public enum DatasetStatus { Draft, Processing, Ready, Failed, Archived }
