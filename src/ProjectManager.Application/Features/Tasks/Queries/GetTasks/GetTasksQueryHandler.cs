using Application.Common.Interfaces;
using Application.Features.Tasks.DTOs;
using AutoMapper;
using MediatR;

namespace Application.Features.Tasks.Queries.GetTasks;

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, IReadOnlyList<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetTasksQueryHandler(ITaskRepository taskRepository, ICurrentUserService currentUserService, IMapper mapper)
    {
        _taskRepository = taskRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TaskDto>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var status = Enum.TryParse<Domain.Enums.TaskStatus>(request.Status, true, out var parsedStatus)
            ? parsedStatus
            : (Domain.Enums.TaskStatus?)null;
        var priority = Enum.TryParse<Domain.Enums.TaskPriority>(request.Priority, true, out var parsedPriority)
            ? parsedPriority
            : (Domain.Enums.TaskPriority?)null;
        var tasks = await _taskRepository.GetTasksByFilterAsync(
            request.ProjectId,
            status,
            priority,
            request.AssignedToId ?? _currentUserService.UserId,
            cancellationToken);

        return _mapper.Map<IReadOnlyList<TaskDto>>(tasks);
    }
}
