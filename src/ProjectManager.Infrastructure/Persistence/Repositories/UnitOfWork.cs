using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly IServiceProvider _serviceProvider;

    public UnitOfWork(AppDbContext context, IServiceProvider serviceProvider)
    {
        _context = context;
        _serviceProvider = serviceProvider;
    }

    public IProjectRepository Projects => _serviceProvider.GetRequiredService<IProjectRepository>();
    public ITaskRepository Tasks => _serviceProvider.GetRequiredService<ITaskRepository>();
    public ICommentRepository Comments => _serviceProvider.GetRequiredService<ICommentRepository>();
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
