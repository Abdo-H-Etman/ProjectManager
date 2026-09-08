using Application.Common.Interfaces;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Task = Domain.Entities.Task;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Infrastructure.Persistence.Repositories;

public class TaskRepository : Repository<Task>, ITaskRepository
{
    public TaskRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Task?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Project)
            .Include(t => t.Comments)
            .Include(t => t.SubTasks)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Task>> GetTasksByFilterAsync(
        Guid? projectId = null,
        TaskStatus? status = null,
        TaskPriority? priority = null,
        Guid? assignedToId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().AsQueryable();

        if (projectId.HasValue)
        {
            query = query.Where(t => t.ProjectId == projectId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(t => t.Priority == priority.Value);
        }

        if (assignedToId.HasValue)
        {
            query = query.Where(t => t.AssignedToId == assignedToId.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Task>> GetTasksByProjectIdAsync(
        Guid projectId,
        TaskStatus? status = null,
        TaskPriority? priority = null,
        Guid? assignedToId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().Where(t => t.ProjectId == projectId);

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(t => t.Priority == priority.Value);
        }

        if (assignedToId.HasValue)
        {
            query = query.Where(t => t.AssignedToId == assignedToId.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Task>> GetUnassignedTasksAsync(
        Guid? projectId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().Where(t => t.AssignedToId == null);

        if (projectId.HasValue)
        {
            query = query.Where(t => t.ProjectId == projectId.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Task>> GetOverdueTasksAsync(
        Guid? assignedToId = null,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var query = _dbSet.AsNoTracking().Where(t => t.DueDate != null && t.DueDate < now && t.Status != TaskStatus.Completed && t.Status != TaskStatus.Cancelled);

        if (assignedToId.HasValue)
        {
            query = query.Where(t => t.AssignedToId == assignedToId.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Task>> GetSubtasksAsync(
        Guid parentTaskId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(t => t.ParentTaskId == parentTaskId)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
