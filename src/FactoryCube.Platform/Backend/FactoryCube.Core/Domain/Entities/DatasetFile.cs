namespace FactoryCube.Core.Domain.Entities;

public class DatasetFile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DatasetId { get; set; }
    public Dataset Dataset { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public FileFormat FileFormat { get; set; } = FileFormat.Csv;
    public long? RowCount { get; set; }
    public string? Checksum { get; set; }
    public UploadStatus UploadStatus { get; set; } = UploadStatus.Pending;
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum FileFormat { Csv, Json, Xlsx }
public enum UploadStatus { Pending, Processing, Success, Failed }
