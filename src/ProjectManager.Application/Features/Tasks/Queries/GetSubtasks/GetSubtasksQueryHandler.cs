using Application.Common.Interfaces;
using Application.Features.Tasks.DTOs;
using AutoMapper;
using Domain.Exceptions;
using MediatR;
using TaskEntity = Domain.Entities.Task;

namespace Application.Features.Tasks.Queries.GetSubtasks;

public class GetSubtasksQueryHandler : IRequestHandler<GetSubtasksQuery, IReadOnlyList<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IMapper _mapper;

    public GetSubtasksQueryHandler(ITaskRepository taskRepository, IMapper mapper)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TaskDto>> Handle(GetSubtasksQuery request, CancellationToken cancellationToken)
    {
        var parentTask = await _taskRepository.GetByIdAsync(request.ParentTaskId, cancellationToken);
        if (parentTask == null)
        {
            throw new NotFoundException(nameof(TaskEntity), request.ParentTaskId);
        }

        var subtasks = await _taskRepository.GetSubtasksAsync(request.ParentTaskId, cancellationToken);

        return _mapper.Map<IReadOnlyList<TaskDto>>(subtasks);
    }
}
