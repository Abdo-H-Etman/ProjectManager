namespace Application.Common.Interfaces;

public interface IUnitOfWork
{
    IProjectRepository Projects { get; }
    ITaskRepository Tasks { get; }
    ICommentRepository Comments { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
