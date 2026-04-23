namespace FactoryCube.Core.Application.DTOs;

public record CreateDatasetRequest(
    Guid ProjectId,
    string Name,
    string? Description,
    string SourceType
);

public record DatasetDto(
    Guid Id,
    Guid ProjectId,
    string Name,
    string? Description,
    string SourceType,
    long RecordCount,
    DateTime? TimeRangeStart,
    DateTime? TimeRangeEnd,
    decimal? QualityScore,
    string Status,
    DateTime CreatedAt
);

public record DatasetFileDto(
    Guid Id,
    Guid DatasetId,
    string FileName,
    long FileSizeBytes,
    string FileFormat,
    long? RowCount,
    string UploadStatus,
    DateTime CreatedAt
);

public record IngestResultDto(
    Guid DatasetId,
    int FilesProcessed,
    long RecordsIngested,
    string Status
);
