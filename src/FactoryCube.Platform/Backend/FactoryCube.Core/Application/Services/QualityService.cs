using FactoryCube.Core.Application.DTOs;
using FactoryCube.Core.Application.Interfaces;
using FactoryCube.Core.Domain.Entities;
using FactoryCube.Core.Domain.Interfaces;

namespace FactoryCube.Core.Application.Services;

public class QualityService : IQualityService
{
    private readonly IRepository<QualityRule> _ruleRepo;
    private readonly IRepository<QualityResult> _resultRepo;

    public QualityService(IRepository<QualityRule> ruleRepo, IRepository<QualityResult> resultRepo)
    {
        _ruleRepo = ruleRepo;
        _resultRepo = resultRepo;
    }

    public async Task<QualityRuleDto> CreateRuleAsync(CreateQualityRuleRequest request, CancellationToken ct = default)
    {
        Enum.TryParse<RuleType>(request.RuleType, true, out var ruleType);
        Enum.TryParse<Severity>(request.Severity, true, out var severity);
        var rule = new QualityRule
        {
            ProjectId = request.ProjectId,
            RuleName = request.RuleName,
            RuleType = ruleType,
            TargetFields = request.TargetFields,
            ConditionExpr = request.ConditionExpr,
            Severity = severity
        };
        await _ruleRepo.AddAsync(rule, ct);
        await _ruleRepo.SaveChangesAsync(ct);
        return new QualityRuleDto(rule.Id, rule.ProjectId, rule.RuleName, rule.RuleType.ToString(),
            rule.TargetFields, rule.ConditionExpr, rule.Severity.ToString(), rule.IsActive);
    }

    public async Task<IReadOnlyList<QualityRuleDto>> GetRulesByProjectAsync(Guid? projectId, CancellationToken ct = default)
    {
        var rules = await _ruleRepo.GetAllAsync(ct);
        if (projectId.HasValue)
            rules = rules.Where(r => r.ProjectId == projectId.Value).ToList();
        return rules.Select(r => new QualityRuleDto(r.Id, r.ProjectId, r.RuleName, r.RuleType.ToString(),
            r.TargetFields, r.ConditionExpr, r.Severity.ToString(), r.IsActive)).ToList();
    }

    public async Task<QualitySummaryDto> RunQualityCheckAsync(Guid datasetId, CancellationToken ct = default)
    {
        // 실제 구현에서는 Python Runner 호출 또는 C# 기반 엔진 실행
        var batchId = Guid.NewGuid();
        var result = new QualityResult
        {
            DatasetId = datasetId,
            CheckBatchId = batchId,
            IssueLevel = IssueLevel.Dataset,
            IssueCount = 0,
            Score = 95.0m,
            Verdict = Verdict.Pass
        };
        await _resultRepo.AddAsync(result, ct);
        await _resultRepo.SaveChangesAsync(ct);
        return new QualitySummaryDto(datasetId, 95.0m, 1, 0, 0,
            new List<QualityResultDto> { new(result.Id, datasetId, result.RuleId, result.CheckTime,
                result.IssueLevel.ToString(), result.AffectedRows, result.IssueCount, result.Score, result.Verdict.ToString()) });
    }

    public async Task<QualitySummaryDto?> GetLatestResultAsync(Guid datasetId, CancellationToken ct = default)
    {
        var results = await _resultRepo.GetAllAsync(ct);
        var latest = results.Where(r => r.DatasetId == datasetId).OrderByDescending(r => r.CheckTime).FirstOrDefault();
        if (latest == null) return null;
        return new QualitySummaryDto(datasetId, latest.Score, 1, 0, 0,
            new List<QualityResultDto> { new(latest.Id, datasetId, latest.RuleId, latest.CheckTime,
                latest.IssueLevel.ToString(), latest.AffectedRows, latest.IssueCount, latest.Score, latest.Verdict.ToString()) });
    }
}
