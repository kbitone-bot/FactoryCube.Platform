namespace FactoryCube.Core.Application.DTOs;

public record QualityRuleDto(
    Guid Id,
    Guid? ProjectId,
    string RuleName,
    string RuleType,
    List<string> TargetFields,
    string ConditionExpr,
    string Severity,
    bool IsActive
);

public record CreateQualityRuleRequest(
    Guid? ProjectId,
    string RuleName,
    string RuleType,
    List<string> TargetFields,
    string ConditionExpr,
    string Severity
);

public record QualityResultDto(
    Guid Id,
    Guid DatasetId,
    Guid? RuleId,
    DateTime CheckTime,
    string IssueLevel,
    long? AffectedRows,
    long IssueCount,
    decimal? Score,
    string Verdict
);

public record QualitySummaryDto(
    Guid DatasetId,
    decimal? OverallScore,
    int PassCount,
    int WarningCount,
    int RejectCount,
    List<QualityResultDto> Results
);
