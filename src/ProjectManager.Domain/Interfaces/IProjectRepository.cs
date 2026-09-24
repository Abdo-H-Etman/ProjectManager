using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetByIdWithTasksAsync(Guid id, CancellationToken cancellationToken = default);
}
