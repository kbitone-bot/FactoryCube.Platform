namespace FactoryCube.Core.Domain.Entities;

public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public EquipmentType EquipmentType { get; set; } = EquipmentType.PLC;
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;

    public ICollection<Dataset> Datasets { get; set; } = new List<Dataset>();
    public ICollection<SchemaMapping> SchemaMappings { get; set; } = new List<SchemaMapping>();
    public ICollection<QualityRule> QualityRules { get; set; } = new List<QualityRule>();
    public ICollection<MlExperiment> Experiments { get; set; } = new List<MlExperiment>();
}

public enum EquipmentType { PLC, TestEquipment, Hybrid }
public enum ProjectStatus { Active, Archived, Deleted }
