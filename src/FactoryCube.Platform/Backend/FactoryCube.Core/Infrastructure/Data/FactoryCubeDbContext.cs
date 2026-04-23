using FactoryCube.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FactoryCube.Core.Infrastructure.Data;

public class FactoryCubeDbContext : DbContext
{
    public FactoryCubeDbContext(DbContextOptions<FactoryCubeDbContext> options) : base(options) { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Dataset> Datasets => Set<Dataset>();
    public DbSet<DatasetFile> DatasetFiles => Set<DatasetFile>();
    public DbSet<SchemaMapping> SchemaMappings => Set<SchemaMapping>();
    public DbSet<RawRecord> RawRecords => Set<RawRecord>();
    public DbSet<NormalizedRecord> NormalizedRecords => Set<NormalizedRecord>();
    public DbSet<QualityRule> QualityRules => Set<QualityRule>();
    public DbSet<QualityResult> QualityResults => Set<QualityResult>();
    public DbSet<SyntheticJob> SyntheticJobs => Set<SyntheticJob>();
    public DbSet<SyntheticDataset> SyntheticDatasets => Set<SyntheticDataset>();
    public DbSet<SyntheticValidation> SyntheticValidations => Set<SyntheticValidation>();
    public DbSet<MlExperiment> MlExperiments => Set<MlExperiment>();
    public DbSet<MlModelRegistry> MlModelRegistries => Set<MlModelRegistry>();
    public DbSet<MlRunMetric> MlRunMetrics => Set<MlRunMetric>();
    public DbSet<PredictionResult> PredictionResults => Set<PredictionResult>();
    public DbSet<DashboardSnapshot> DashboardSnapshots => Set<DashboardSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PostgreSQL JSON 컬럼 매핑
        modelBuilder.Entity<SchemaMapping>().Property(x => x.Mappings).HasColumnType("jsonb");
        modelBuilder.Entity<RawRecord>().Property(x => x.RawData).HasColumnType("jsonb");
        modelBuilder.Entity<NormalizedRecord>().Property(x => x.TagValues).HasColumnType("jsonb");
        modelBuilder.Entity<NormalizedRecord>().Property(x => x.QualityFlags).HasColumnType("jsonb");
        modelBuilder.Entity<QualityResult>().Property(x => x.SampleIssues).HasColumnType("jsonb");
        modelBuilder.Entity<QualityResult>().Property(x => x.Details).HasColumnType("jsonb");
        modelBuilder.Entity<SyntheticJob>().Property(x => x.ScenarioConfig).HasColumnType("jsonb");
        modelBuilder.Entity<SyntheticDataset>().Property(x => x.ScenarioMix).HasColumnType("jsonb");
        modelBuilder.Entity<SyntheticValidation>().Property(x => x.Details).HasColumnType("jsonb");
        modelBuilder.Entity<MlExperiment>().Property(x => x.FeatureConfig).HasColumnType("jsonb");
        modelBuilder.Entity<MlExperiment>().Property(x => x.Hyperparameters).HasColumnType("jsonb");
        modelBuilder.Entity<MlExperiment>().Property(x => x.TrainConfig).HasColumnType("jsonb");
        modelBuilder.Entity<MlModelRegistry>().Property(x => x.Metrics).HasColumnType("jsonb");
        modelBuilder.Entity<PredictionResult>().Property(x => x.FeatureSnapshot).HasColumnType("jsonb");
        modelBuilder.Entity<DashboardSnapshot>().Property(x => x.Kpis).HasColumnType("jsonb");
        modelBuilder.Entity<DashboardSnapshot>().Property(x => x.ChartData).HasColumnType("jsonb");

        // 인덱스 및 제약조건
        modelBuilder.Entity<Dataset>().HasIndex(x => new { x.ProjectId, x.Status });
        modelBuilder.Entity<DatasetFile>().HasIndex(x => x.DatasetId);
        modelBuilder.Entity<RawRecord>().HasIndex(x => new { x.DatasetId, x.SeqNo });
        modelBuilder.Entity<NormalizedRecord>().HasIndex(x => new { x.DatasetId, x.RecordTime });
        modelBuilder.Entity<QualityResult>().HasIndex(x => new { x.DatasetId, x.CheckTime });
        modelBuilder.Entity<SyntheticJob>().HasIndex(x => new { x.ProjectId, x.Status });
        modelBuilder.Entity<MlExperiment>().HasIndex(x => new { x.ProjectId, x.Status });
        modelBuilder.Entity<PredictionResult>().HasIndex(x => new { x.ExperimentId, x.PredictionTime });
        modelBuilder.Entity<DashboardSnapshot>().HasIndex(x => new { x.ProjectId, x.SnapshotTime });
    }
}
