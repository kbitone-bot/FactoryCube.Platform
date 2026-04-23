using FactoryCube.Core.Application.DTOs;
using FactoryCube.Core.Application.Interfaces;
using FactoryCube.Core.Domain.Entities;
using FactoryCube.Core.Domain.Interfaces;
using System.IO;

namespace FactoryCube.Core.Application.Services;

public class DatasetService : IDatasetService
{
    private readonly IDatasetRepository _datasetRepo;
    private readonly IRepository<DatasetFile> _fileRepo;
    private readonly IRepository<RawRecord> _rawRepo;
    private readonly string _uploadBasePath;

    public DatasetService(
        IDatasetRepository datasetRepo,
        IRepository<DatasetFile> fileRepo,
        IRepository<RawRecord> rawRepo,
        IConfiguration config)
    {
        _datasetRepo = datasetRepo;
        _fileRepo = fileRepo;
        _rawRepo = rawRepo;
        _uploadBasePath = config["Storage:UploadPath"] ?? Path.Combine("data", "uploads");
        Directory.CreateDirectory(_uploadBasePath);
    }

    public async Task<DatasetDto> CreateAsync(CreateDatasetRequest request, CancellationToken ct = default)
    {
        Enum.TryParse<SourceType>(request.SourceType, true, out var srcType);
        var ds = new Dataset
        {
            ProjectId = request.ProjectId,
            Name = request.Name,
            Description = request.Description,
            SourceType = srcType
        };
        await _datasetRepo.AddAsync(ds, ct);
        await _datasetRepo.SaveChangesAsync(ct);
        return MapToDto(ds);
    }

    public async Task<DatasetDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var ds = await _datasetRepo.GetByIdAsync(id, ct);
        return ds == null ? null : MapToDto(ds);
    }

    public async Task<IReadOnlyList<DatasetDto>> GetByProjectAsync(Guid projectId, CancellationToken ct = default)
    {
        var list = await _datasetRepo.GetByProjectAsync(projectId, ct);
        return list.Select(MapToDto).ToList();
    }

    public async Task<DatasetFileDto> UploadFileAsync(Guid datasetId, IFormFile file, CancellationToken ct = default)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fmt = ext switch { ".csv" => FileFormat.Csv, ".json" => FileFormat.Json, ".xlsx" => FileFormat.Xlsx, _ => FileFormat.Csv };
        var folder = Path.Combine(_uploadBasePath, datasetId.ToString());
        Directory.CreateDirectory(folder);
        var path = Path.Combine(folder, $"{Guid.NewGuid()}_{file.FileName}");
        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream, ct);

        var df = new DatasetFile
        {
            DatasetId = datasetId,
            FileName = file.FileName,
            FilePath = path,
            FileSizeBytes = file.Length,
            FileFormat = fmt,
            UploadStatus = UploadStatus.Success
        };
        await _fileRepo.AddAsync(df, ct);
        await _fileRepo.SaveChangesAsync(ct);
        return new DatasetFileDto(df.Id, df.DatasetId, df.FileName, df.FileSizeBytes, df.FileFormat.ToString(), df.RowCount, df.UploadStatus.ToString(), df.CreatedAt);
    }

    public async Task<IngestResultDto> IngestAsync(Guid datasetId, CancellationToken ct = default)
    {
        var ds = await _datasetRepo.GetByIdWithFilesAsync(datasetId, ct);
        if (ds == null) throw new InvalidOperationException("Dataset not found");

        await _datasetRepo.UpdateStatusAsync(datasetId, DatasetStatus.Processing, ct);
        long total = 0;
        int filesProcessed = 0;
        foreach (var f in ds.Files.Where(x => x.UploadStatus == UploadStatus.Success))
        {
            // 실제 구현에서는 CSV/JSON/XLSX 파서를 사용
            // 여기서는 개념적으로 SeqNo 증가만 수행
            for (int i = 0; i < 1000; i++) // 샘플
            {
                var rr = new RawRecord
                {
                    DatasetId = datasetId,
                    SourceFileId = f.Id,
                    RawData = "{}",
                    SeqNo = total + i
                };
                await _rawRepo.AddAsync(rr, ct);
            }
            total += 1000;
            filesProcessed++;
        }
        await _rawRepo.SaveChangesAsync(ct);
        await _datasetRepo.UpdateStatusAsync(datasetId, DatasetStatus.Ready, ct);
        return new IngestResultDto(datasetId, filesProcessed, total, "Ready");
    }

    private static DatasetDto MapToDto(Dataset d) => new(
        d.Id, d.ProjectId, d.Name, d.Description, d.SourceType.ToString(),
        d.RecordCount, d.TimeRangeStart, d.TimeRangeEnd, d.QualityScore,
        d.Status.ToString(), d.CreatedAt);
}
