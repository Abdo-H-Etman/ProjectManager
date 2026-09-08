using Application.Features.Projects.DTOs;
using MediatR;

namespace Application.Features.Projects.Queries.GetProjectSummary;

public record GetProjectSummaryQuery(Guid Id) : IRequest<ProjectSummaryDto>;
