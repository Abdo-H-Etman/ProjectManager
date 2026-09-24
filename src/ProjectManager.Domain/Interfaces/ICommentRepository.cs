using Domain.Entities;

namespace Application.Common.Interfaces;

public interface ICommentRepository : IRepository<Comment>
{
    Task<IReadOnlyList<Comment>> GetCommentsByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
}
