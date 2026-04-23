using FactoryCube.Core.Application.DTOs;
using FactoryCube.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FactoryCube.Core.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _svc;

    public DashboardController(IDashboardService svc)
    {
        _svc = svc;
    }

    [HttpGet("projects/{projectId:guid}/snapshot")]
    public async Task<ActionResult<DashboardSnapshotDto>> GetSnapshot(Guid projectId, CancellationToken ct)
        => Ok(await _svc.GetLatestSnapshotAsync(projectId, ct));

    [HttpGet("projects/{projectId:guid}/equipment-status")]
    public async Task<ActionResult<IReadOnlyList<EquipmentStatusSummaryDto>>> GetEquipmentStatus(Guid projectId, CancellationToken ct)
        => Ok(await _svc.GetEquipmentStatusAsync(projectId, ct));
}
