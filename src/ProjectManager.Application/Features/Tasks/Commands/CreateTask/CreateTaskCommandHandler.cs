using Application.Common.Interfaces;
using Application.Features.Tasks.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using TaskEntity = Domain.Entities.Task;

namespace Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateTaskCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var projectExists = await _unitOfWork.Projects.ExistsAsync(request.ProjectId, cancellationToken);
        if (!projectExists)
        {
            throw new NotFoundException(nameof(Project), request.ProjectId);
        }

        var createdById = _currentUserService.UserId ?? request.CreatedById;

        var task = new TaskEntity
        {
            ProjectId = request.ProjectId,
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            Status = request.Status,
            DueDate = request.DueDate,
            StartDate = request.StartDate,
            AssignedToId = request.AssignedToId,
            AssignedAt = request.AssignedToId.HasValue ? DateTime.UtcNow : null,
            CreatedById = createdById,
            ParentTaskId = request.ParentTaskId,
            EstimatedHours = request.EstimatedHours,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Tasks.AddAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TaskDto
        {
            Id = task.Id,
            ProjectId = task.ProjectId,
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority,
            Status = task.Status,
            DueDate = task.DueDate,
            StartDate = task.StartDate,
            CompletedAt = task.CompletedAt,
            AssignedToId = task.AssignedToId,
            AssignedAt = task.AssignedAt,
            CreatedById = task.CreatedById,
            ParentTaskId = task.ParentTaskId,
            EstimatedHours = task.EstimatedHours,
            ActualHours = task.ActualHours,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}
