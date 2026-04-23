using FactoryCube.Core.Domain.Entities;

namespace FactoryCube.Core.Domain.Interfaces;

public interface IProjectRepository : IRepository<Project>
{
    Task<IReadOnlyList<Project>> GetByEquipmentTypeAsync(string equipmentType, CancellationToken ct = default);
    Task<Project?> GetByIdWithDatasetsAsync(Guid id, CancellationToken ct = default);
}
