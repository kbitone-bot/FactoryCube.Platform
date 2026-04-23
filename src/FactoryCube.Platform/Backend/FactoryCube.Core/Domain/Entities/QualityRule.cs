namespace FactoryCube.Core.Domain.Entities;

public class QualityRule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public RuleType RuleType { get; set; } = RuleType.Schema;
    public List<string> TargetFields { get; set; } = new();
    public string ConditionExpr { get; set; } = string.Empty;
    public Severity Severity { get; set; } = Severity.Warning;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum RuleType { Schema, Value, Timeseries, Domain }
public enum Severity { Info, Warning, Critical }
