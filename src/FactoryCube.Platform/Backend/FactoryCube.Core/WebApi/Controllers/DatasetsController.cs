using FactoryCube.Core.Application.DTOs;
using FactoryCube.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FactoryCube.Core.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DatasetsController : ControllerBase
{
    private readonly IDatasetService _svc;

    public DatasetsController(IDatasetService svc)
    {
        _svc = svc;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DatasetDto>>> GetByProject([FromQuery] Guid projectId, CancellationToken ct)
        => Ok(await _svc.GetByProjectAsync(projectId, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DatasetDto>> GetById(Guid id, CancellationToken ct)
    {
        var dto = await _svc.GetByIdAsync(id, ct);
        return dto == null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<DatasetDto>> Create([FromBody] CreateDatasetRequest request, CancellationToken ct)
    {
        var dto = await _svc.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPost("{id:guid}/upload")]
    public async Task<ActionResult<DatasetFileDto>> Upload(Guid id, IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0) return BadRequest("No file uploaded");
        var dto = await _svc.UploadFileAsync(id, file, ct);
        return Ok(dto);
    }

    [HttpPost("{id:guid}/ingest")]
    public async Task<ActionResult<IngestResultDto>> Ingest(Guid id, CancellationToken ct)
    {
        var result = await _svc.IngestAsync(id, ct);
        return Ok(result);
    }
}
