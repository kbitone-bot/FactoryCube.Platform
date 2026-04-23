using FactoryCube.Core.Application.DTOs;
using FactoryCube.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FactoryCube.Core.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QualityController : ControllerBase
{
    private readonly IQualityService _svc;

    public QualityController(IQualityService svc)
    {
        _svc = svc;
    }

    [HttpGet("rules")]
    public async Task<ActionResult<IReadOnlyList<QualityRuleDto>>> GetRules([FromQuery] Guid? projectId, CancellationToken ct)
        => Ok(await _svc.GetRulesByProjectAsync(projectId, ct));

    [HttpPost("rules")]
    public async Task<ActionResult<QualityRuleDto>> CreateRule([FromBody] CreateQualityRuleRequest request, CancellationToken ct)
    {
        var dto = await _svc.CreateRuleAsync(request, ct);
        return Ok(dto);
    }

    [HttpPost("datasets/{datasetId:guid}/check")]
    public async Task<ActionResult<QualitySummaryDto>> RunCheck(Guid datasetId, CancellationToken ct)
        => Ok(await _svc.RunQualityCheckAsync(datasetId, ct));

    [HttpGet("datasets/{datasetId:guid}/latest")]
    public async Task<ActionResult<QualitySummaryDto>> GetLatest(Guid datasetId, CancellationToken ct)
    {
        var dto = await _svc.GetLatestResultAsync(datasetId, ct);
        return dto == null ? NotFound() : Ok(dto);
    }
}
