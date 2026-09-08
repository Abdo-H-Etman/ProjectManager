using Application.Features.Dashboard.DTOs;
using MediatR;

namespace Application.Features.Dashboard.Queries.GetMyDashboard;

public record GetMyDashboardQuery : IRequest<DashboardDto>;
