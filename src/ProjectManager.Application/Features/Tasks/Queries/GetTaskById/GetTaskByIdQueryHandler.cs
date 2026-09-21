using Application.Common.Interfaces;
using Application.Features.Comments.DTOs;
using Application.Features.Tasks.DTOs;
using AutoMapper;
using Domain.Exceptions;
using MediatR;
using TaskEntity = Domain.Entities.Task;

namespace Application.Features.Tasks.Queries.GetTaskById;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskDetailDto>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IMapper _mapper;

    public GetTaskByIdQueryHandler(ITaskRepository taskRepository, IMapper mapper)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
    }

    public async Task<TaskDetailDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException(nameof(Task), request.Id);
        }

        var taskDto = _mapper.Map<TaskDetailDto>(task);
        taskDto.Comments = task.Comments?.Select(comment => new CommentDto
        {
            Id = comment.Id,
            TaskId = comment.TaskId,
            AuthorId = comment.AuthorId,
            ParentCommentId = comment.ParentCommentId,
            Content = comment.Content,
            IsEdited = comment.IsEdited,
            EditedAt = comment.EditedAt,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        }).ToList() ?? [];

        return taskDto;
    }
}
