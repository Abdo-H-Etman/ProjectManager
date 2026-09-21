using Application.Common.Interfaces;
using Application.Features.Tasks.DTOs;
using AutoMapper;
using MediatR;

namespace Application.Features.Tasks.Queries.GetUnassignedTasks;

public class GetUnassignedTasksQueryHandler : IRequestHandler<GetUnassignedTasksQuery, IReadOnlyList<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IMapper _mapper;

    public GetUnassignedTasksQueryHandler(ITaskRepository taskRepository, IMapper mapper)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TaskDto>> Handle(GetUnassignedTasksQuery request, CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetUnassignedTasksAsync(request.ProjectId, cancellationToken);

        return _mapper.Map<IReadOnlyList<TaskDto>>(tasks);
    }
}
