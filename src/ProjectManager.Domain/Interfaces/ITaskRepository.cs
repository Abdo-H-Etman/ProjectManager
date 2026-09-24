using Domain.Enums;
using TaskEntity = Domain.Entities.Task;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Common.Interfaces;

public interface ITaskRepository : IRepository<TaskEntity>
{
    Task<TaskEntity?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaskEntity>> GetTasksByFilterAsync(
        Guid? projectId = null,
        TaskStatus? status = null,
        TaskPriority? priority = null,
        Guid? assignedToId = null,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaskEntity>> GetTasksByProjectIdAsync(
        Guid projectId,
        TaskStatus? status = null,
        TaskPriority? priority = null,
        Guid? assignedToId = null,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaskEntity>> GetUnassignedTasksAsync(
        Guid? projectId = null,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaskEntity>> GetOverdueTasksAsync(
        Guid? assignedToId = null,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaskEntity>> GetSubtasksAsync(
        Guid parentTaskId,
        CancellationToken cancellationToken = default);
}
