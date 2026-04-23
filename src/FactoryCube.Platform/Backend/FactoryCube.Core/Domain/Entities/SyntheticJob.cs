namespace FactoryCube.Core.Domain.Entities;

public class SyntheticJob
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string JobName { get; set; } = string.Empty;
    public string ScenarioConfig { get; set; } = "{}"; // JSON
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int EquipmentCount { get; set; } = 1;
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public int ProgressPct { get; set; }
    public Guid? OutputDatasetId { get; set; }
    public Dataset? OutputDataset { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}

public enum JobStatus { Pending, Running, Completed, Failed, Cancelled }
