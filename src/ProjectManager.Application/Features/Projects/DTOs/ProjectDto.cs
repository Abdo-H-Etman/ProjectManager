using Domain.Enums;

namespace Application.Features.Projects.DTOs;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; }
    public Guid OwnerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ProjectDetailDto : ProjectDto
{
    public int TaskCount { get; set; }
    public int CompletedTaskCount { get; set; }
}
