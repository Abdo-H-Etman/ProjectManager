using Application.Common.Interfaces;
using Application.Features.Tasks.DTOs;
using AutoMapper;
using MediatR;

namespace Application.Features.Tasks.Queries.GetOverdueTasks;

public class GetOverdueTasksQueryHandler : IRequestHandler<GetOverdueTasksQuery, IReadOnlyList<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetOverdueTasksQueryHandler(ITaskRepository taskRepository, ICurrentUserService currentUserService, IMapper mapper)
    {
        _taskRepository = taskRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TaskDto>> Handle(GetOverdueTasksQuery request, CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetOverdueTasksAsync(_currentUserService.UserId, cancellationToken);

        return _mapper.Map<IReadOnlyList<TaskDto>>(tasks);
    }
}
