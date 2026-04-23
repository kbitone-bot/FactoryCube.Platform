using FactoryCube.Core.Application.DTOs;

namespace FactoryCube.Core.Application.Interfaces;

public interface IDatasetService
{
    Task<DatasetDto> CreateAsync(CreateDatasetRequest request, CancellationToken ct = default);
    Task<DatasetDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<DatasetDto>> GetByProjectAsync(Guid projectId, CancellationToken ct = default);
    Task<DatasetFileDto> UploadFileAsync(Guid datasetId, IFormFile file, CancellationToken ct = default);
    Task<IngestResultDto> IngestAsync(Guid datasetId, CancellationToken ct = default);
}
