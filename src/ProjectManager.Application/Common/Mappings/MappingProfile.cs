using AutoMapper;
using Application.Features.Projects.DTOs;
using Application.Features.Tasks.DTOs;
using ProjectEntity = Domain.Entities.Project;
using TaskEntity = Domain.Entities.Task;

namespace Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ProjectEntity, ProjectDto>()
            .ForMember(destination => destination.Status,
                options => options.MapFrom(source => source.Status.ToString()));

        CreateMap<ProjectEntity, ProjectDetailDto>()
            .IncludeBase<ProjectEntity, ProjectDto>()
            .ForMember(destination => destination.TaskCount,
                options => options.MapFrom(source => source.Tasks == null ? 0 : source.Tasks.Count))
            .ForMember(destination => destination.CompletedTaskCount,
                options => options.MapFrom(source => source.Tasks == null
                    ? 0
                    : source.Tasks.Count(task => task.Status == Domain.Enums.TaskStatus.Completed)));

        CreateMap<TaskEntity, TaskDto>()
            .ForMember(destination => destination.Priority,
                options => options.MapFrom(source => source.Priority.ToString()))
            .ForMember(destination => destination.Status,
                options => options.MapFrom(source => source.Status.ToString()));

        CreateMap<TaskEntity, TaskDetailDto>()
            .IncludeBase<TaskEntity, TaskDto>()
            .ForMember(destination => destination.ProjectName,
                options => options.MapFrom(source => source.Project == null ? null : source.Project.Name))
            .ForMember(destination => destination.CommentCount,
                options => options.MapFrom(source => source.Comments == null ? 0 : source.Comments.Count))
            .ForMember(destination => destination.SubTaskCount,
                options => options.MapFrom(source => source.SubTasks == null ? 0 : source.SubTasks.Count));
    }
}