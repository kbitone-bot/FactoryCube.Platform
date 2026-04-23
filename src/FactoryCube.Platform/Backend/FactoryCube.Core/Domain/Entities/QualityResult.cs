namespace FactoryCube.Core.Domain.Entities;

public class QualityResult
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DatasetId { get; set; }
    public Dataset Dataset { get; set; } = null!;
    public Guid? RuleId { get; set; }
    public QualityRule? Rule { get; set; }
    public Guid CheckBatchId { get; set; }
    public DateTime CheckTime { get; set; } = DateTime.UtcNow;
    public IssueLevel IssueLevel { get; set; } = IssueLevel.Row;
    public long? AffectedRows { get; set; }
    public long IssueCount { get; set; }
    public string? SampleIssues { get; set; } // JSON
    public decimal? Score { get; set; }
    public Verdict Verdict { get; set; } = Verdict.Pass;
    public string? Details { get; set; } // JSON
}

public enum IssueLevel { Row, Batch, Dataset }
public enum Verdict { Pass, Warning, Reject }
