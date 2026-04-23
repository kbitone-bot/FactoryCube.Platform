using FactoryCube.Core.Application.DTOs;
using FactoryCube.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FactoryCube.Core.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MlController : ControllerBase
{
    private readonly IMlService _svc;

    public MlController(IMlService svc)
    {
        _svc = svc;
    }

    [HttpGet("projects/{projectId:guid}/experiments")]
    public async Task<ActionResult<IReadOnlyList<ExperimentDto>>> GetExperiments(Guid projectId, CancellationToken ct)
        => Ok(await _svc.GetExperimentsByProjectAsync(projectId, ct));

    [HttpGet("experiments/{id:guid}")]
    public async Task<ActionResult<ExperimentDto>> GetExperiment(Guid id, CancellationToken ct)
    {
        var dto = await _svc.GetExperimentByIdAsync(id, ct);
        return dto == null ? NotFound() : Ok(dto);
    }

    [HttpPost("experiments")]
    public async Task<ActionResult<ExperimentDto>> CreateExperiment([FromBody] CreateExperimentRequest request, CancellationToken ct)
    {
        var dto = await _svc.CreateExperimentAsync(request, ct);
        return CreatedAtAction(nameof(GetExperiment), new { id = dto.Id }, dto);
    }

    [HttpPost("experiments/{id:guid}/train")]
    public async Task<IActionResult> StartTraining(Guid id, CancellationToken ct)
    {
        var ok = await _svc.StartTrainingAsync(id, ct);
        return ok ? Accepted() : NotFound();
    }

    [HttpPost("experiments/{id:guid}/infer")]
    public async Task<IActionResult> RunInference(Guid id, [FromQuery] Guid datasetId, CancellationToken ct)
    {
        var ok = await _svc.RunInferenceAsync(id, datasetId, ct);
        return ok ? Accepted() : NotFound();
    }

    [HttpGet("experiments/{id:guid}/predictions")]
    public async Task<ActionResult<IReadOnlyList<PredictionDto>>> GetPredictions(Guid id, CancellationToken ct)
        => Ok(await _svc.GetPredictionsAsync(id, ct));
}
