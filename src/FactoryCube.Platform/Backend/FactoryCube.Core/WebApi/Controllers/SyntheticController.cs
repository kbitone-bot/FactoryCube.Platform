using FactoryCube.Core.Application.DTOs;
using FactoryCube.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FactoryCube.Core.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SyntheticController : ControllerBase
{
    private readonly ISyntheticService _svc;

    public SyntheticController(ISyntheticService svc)
    {
        _svc = svc;
    }

    [HttpGet("projects/{projectId:guid}/jobs")]
    public async Task<ActionResult<IReadOnlyList<SyntheticJobDto>>> GetJobs(Guid projectId, CancellationToken ct)
        => Ok(await _svc.GetJobsByProjectAsync(projectId, ct));

    [HttpGet("jobs/{id:guid}")]
    public async Task<ActionResult<SyntheticJobDto>> GetJob(Guid id, CancellationToken ct)
    {
        var dto = await _svc.GetJobByIdAsync(id, ct);
        return dto == null ? NotFound() : Ok(dto);
    }

    [HttpPost("jobs")]
    public async Task<ActionResult<SyntheticJobDto>> CreateJob([FromBody] CreateSyntheticJobRequest request, CancellationToken ct)
    {
        var dto = await _svc.CreateJobAsync(request, ct);
        return CreatedAtAction(nameof(GetJob), new { id = dto.Id }, dto);
    }

    [HttpGet("validations/{syntheticDatasetId:guid}")]
    public async Task<ActionResult<IReadOnlyList<SyntheticValidationDto>>> GetValidations(Guid syntheticDatasetId, CancellationToken ct)
        => Ok(await _svc.GetValidationsAsync(syntheticDatasetId, ct));
}
