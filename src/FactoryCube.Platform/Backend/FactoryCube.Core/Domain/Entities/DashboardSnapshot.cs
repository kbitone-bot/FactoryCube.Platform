namespace FactoryCube.Core.Domain.Entities;

public class DashboardSnapshot
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public DateTime SnapshotTime { get; set; } = DateTime.UtcNow;
    public string SnapshotType { get; set; } = string.Empty;
    public string Kpis { get; set; } = "{}"; // JSON
    public string? ChartData { get; set; } // JSON
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
