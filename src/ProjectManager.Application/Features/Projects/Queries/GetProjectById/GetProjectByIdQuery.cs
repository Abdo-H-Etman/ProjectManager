using Application.Features.Projects.DTOs;
using MediatR;

namespace Application.Features.Projects.Queries.GetProjectById;

public record GetProjectByIdQuery(Guid Id) : IRequest<ProjectDetailDto>;
