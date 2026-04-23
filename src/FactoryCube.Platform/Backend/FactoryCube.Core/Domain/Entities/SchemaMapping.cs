namespace FactoryCube.Core.Domain.Entities;

public class SchemaMapping
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string MappingName { get; set; } = string.Empty;
    public string SourceFormat { get; set; } = string.Empty;
    public string Mappings { get; set; } = "{}"; // JSON
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
