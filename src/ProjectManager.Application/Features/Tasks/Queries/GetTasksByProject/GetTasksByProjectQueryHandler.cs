using Application.Common.Interfaces;
using Application.Features.Tasks.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Tasks.Queries.GetTasksByProject;

public class GetTasksByProjectQueryHandler : IRequestHandler<GetTasksByProjectQuery, IReadOnlyList<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;

    public GetTasksByProjectQueryHandler(ITaskRepository taskRepository, IProjectRepository projectRepository, IMapper mapper)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TaskDto>> Handle(GetTasksByProjectQuery request, CancellationToken cancellationToken)
    {
        var projectExists = await _projectRepository.ExistsAsync(request.ProjectId, cancellationToken);
        if (!projectExists)
        {
            throw new NotFoundException(nameof(Project), request.ProjectId);
        }

        var status = Enum.TryParse<TaskStatus>(request.Status, true, out var parsedStatus)
            ? parsedStatus
            : (TaskStatus?)null;
        var priority = Enum.TryParse<Domain.Enums.TaskPriority>(request.Priority, true, out var parsedPriority)
            ? parsedPriority
            : (Domain.Enums.TaskPriority?)null;

        var tasks = await _taskRepository.GetTasksByProjectIdAsync(
            request.ProjectId,
            status,
            priority,
            request.AssignedToId,
            cancellationToken);

        return _mapper.Map<IReadOnlyList<TaskDto>>(tasks);
    }
}
